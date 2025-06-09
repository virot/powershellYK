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

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Fido2.Commands;
using powershellYK.Exceptions;
using System.Diagnostics;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyFIDO2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetYubikeyFIDO2Cmdlet : PSCmdlet
    {
        // Track YubiKey removal and insertion events
        private bool _yubiKeyRemoved = false;
        private bool _yubiKeyArrived = false;

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
                Console.WriteLine("Remove and re-insert the YubiKey to perform the reset...");

                // Set up YubiKey device monitoring
                var yubiKeyDeviceListener = YubiKeyDeviceListener.Instance;
                // Use a stopwatch to make sure we wont get stuck in an infinite loop.
                Stopwatch stopwatch = new Stopwatch();

                // Register event handlers for device removal and insertion
                yubiKeyDeviceListener.Removed += YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived += YubiKeyArrived;

                // Wait for YubiKey removal (10 second timeout)
                stopwatch.Start();
                while (!_yubiKeyRemoved)
                {
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
                    System.Threading.Thread.Sleep(100);
                    if (stopwatch.Elapsed.TotalSeconds > 10)
                    {
                        throw new Exception("YubiKey was not inserted within 10 seconds. Reset aborted.");
                    }
                }

                // Unregister event handlers
                yubiKeyDeviceListener.Removed -= YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived -= YubiKeyArrived;

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
        }

        // Event handler for YubiKey removal
        private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (YubiKeyModule._yubikey!.SerialNumber == e.Device.SerialNumber)
            {
                _yubiKeyRemoved = true;
            }
        }

        // Event handler for YubiKey insertion
        private void YubiKeyArrived(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (YubiKeyModule._yubikey!.SerialNumber == e.Device.SerialNumber)
            {
                _yubiKeyArrived = true;
            }
        }
    }
}