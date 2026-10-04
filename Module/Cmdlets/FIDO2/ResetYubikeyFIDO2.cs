/// <summary>
/// Resets the FIDO2 applet on a YubiKey to factory settings.
/// Deletes all FIDO credentials, including FIDO U2F credentials.
/// Requires physical interaction with the YubiKey (removal and reinsertion).
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// Note: This operation cannot be undone.
/// 
/// .EXAMPLE
/// Reset-YubiKeyFIDO2
/// Resets the FIDO2 applet on the connected YubiKey
/// 
/// .EXAMPLE
/// Reset-YubiKeyFIDO2 -Confirm:$false
/// Resets the FIDO2 applet without confirmation prompt
/// </summary>

using System.Diagnostics;
using System.Management.Automation;
using powershellYK.support;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Commands;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyFIDO2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetYubikeyFIDO2Cmdlet : PSCmdlet
    {
        // Track YubiKey removal and insertion events
        private bool _yubiKeyRemoved = false;
        private bool _yubiKeyArrived = false;
        private int? _expectedSerial;
        private IYubiKeyDevice? _reinsertedDevice;
        private string? _selectionError;

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }

            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Add Bio MPE check
            // TODO: @virot can/should we use our helper instead here?
            var formFactor = YubiKeyModule._yubikey!.FormFactor;
            var capabilities = YubiKeyModule._yubikey.AvailableUsbCapabilities;

            if ((formFactor == FormFactor.UsbABiometricKeychain || formFactor == FormFactor.UsbCBiometricKeychain) && capabilities.HasFlag(YubiKeyCapabilities.Piv))
            {
                throw new Exception("YubiKey Bio Multi-Protocol Edition (MPE) detected. Reset using 'Reset-YubiKey' instead!");
            }

            if (ShouldProcess("This will delete all FIDO credentials, including FIDO U2F credentials, and restore factory settings. Proceed?", "This will delete all FIDO credentials, including FIDO U2F credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                _expectedSerial = YubiKeyModule._yubikey!.SerialNumber;

                // Keys without a serial number cannot be matched safely when multiple YubiKeys are present
                if (_expectedSerial is null && YubiKeyDevice.FindAll().Count() > 1)
                {
                    throw new Exception("The selected YubiKey has no serial number and multiple YubiKeys are connected. Disconnect other YubiKeys and try again.");
                }

                Console.WriteLine("Remove and re-insert the YubiKey to perform the reset...");

                // Set up YubiKey device monitoring
                var yubiKeyDeviceListener = YubiKeyDeviceListener.Instance;
                // Use a stopwatch to make sure we wont get stuck in an infinite loop.
                Stopwatch stopwatch = new Stopwatch();

                try
                {
                    // Register event handlers for device removal and insertion
                    yubiKeyDeviceListener.Removed += YubiKeyRemoved;
                    yubiKeyDeviceListener.Arrived += YubiKeyArrived;

                    // Wait for YubiKey removal (10 second timeout)
                    stopwatch.Start();
                    while (!_yubiKeyRemoved)
                    {
                        if (_selectionError is not null)
                        {
                            throw new Exception(_selectionError);
                        }
                        System.Threading.Thread.Sleep(100);
                        if (stopwatch.Elapsed.TotalSeconds > 10)
                        {
                            throw new Exception("YubiKey was not removed within 10 seconds. Reset aborted.");
                        }
                    }

                    // Wait for YubiKey reinsertion (10 second timeout)
                    stopwatch.Restart();
                    while (!_yubiKeyArrived)
                    {
                        if (_selectionError is not null)
                        {
                            throw new Exception(_selectionError);
                        }
                        System.Threading.Thread.Sleep(100);
                        if (stopwatch.Elapsed.TotalSeconds > 10)
                        {
                            throw new Exception("YubiKey was not inserted within 10 seconds. Reset aborted.");
                        }
                    }

                    if (_reinsertedDevice is null)
                    {
                        throw new Exception("YubiKey was reinserted but the device could not be identified. Reset aborted.");
                    }

                    // Replace the stale pre-removal device reference with the freshly arrived device
                    YubiKeyModule._yubikey = (YubiKeyDevice)_reinsertedDevice;

                    // Perform the reset operation
                    using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                    {
                        ResetCommand resetCommand = new ResetCommand();
                        Console.WriteLine("Touch the YubiKey...");

                        ResetResponse reply = fido2Session.Connection.SendCommand(resetCommand);
                        switch (reply.Status)
                        {
                            default:
                                throw new Exception("Unknown status. (Update ResetYubikeyFIDO2Cmdlet)");

                            case ResponseStatus.Failed:
                                throw new Exception("Please touch the YubiKey to complete reset.");

                            case ResponseStatus.ConditionsNotSatisfied:
                                throw new Exception("Failed to reset, YubiKey needs to be reinserted within 5 seconds.");

                            case ResponseStatus.Success:
                                break;
                        }

                        // Clear FIDO2 PIN after successful reset
                        YubiKeyModule._fido2PIN = null;
                        WriteInformation("YubiKey FIDO applet successfully reset.", new string[] { "FIDO2", "Reset" });
                    }
                }
                finally
                {
                    yubiKeyDeviceListener.Removed -= YubiKeyRemoved;
                    yubiKeyDeviceListener.Arrived -= YubiKeyArrived;
                }
            }
        }

        // Event handler for YubiKey removal
        private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (IsExpectedDevice(e.Device))
            {
                _yubiKeyRemoved = true;
            }
        }

        // Event handler for YubiKey insertion
        private void YubiKeyArrived(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (_expectedSerial is not null)
            {
                if (e.Device.SerialNumber == _expectedSerial)
                {
                    _reinsertedDevice = e.Device;
                    _yubiKeyArrived = true;
                }
                return;
            }

            // Null-serial keys: only accept when a single YubiKey is present after reinsertion
            if (YubiKeyDevice.FindAll().Count() == 1)
            {
                _reinsertedDevice = e.Device;
                _yubiKeyArrived = true;
                return;
            }

            _selectionError = "The selected YubiKey has no serial number and multiple YubiKeys are connected after reinsertion. Disconnect other YubiKeys and try again.";
        }

        // Match the expected YubiKey by serial, or accept any removal when serial is absent and uniqueness was validated at start
        private bool IsExpectedDevice(IYubiKeyDevice device)
        {
            if (_expectedSerial is not null)
            {
                return device.SerialNumber == _expectedSerial;
            }

            // With a null serial we already required a single connected YubiKey before waiting
            return true;
        }
    }
}
