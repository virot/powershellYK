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
        private bool _yubiKeyRemoved = false;
        private bool _yubiKeyArrived = false;

        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }

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

        protected override void ProcessRecord()
        {
            if (ShouldProcess("This will delete all FIDO credentials, including FIDO U2F credentials, and restore factory settings. Proceed?", "This will delete all FIDO credentials, including FIDO U2F credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                Console.WriteLine("Remove and re-insert the YubiKey to perform the reset...");

                // Set up the YubiKeyDeviceListener
                // This must not be disposed of.
                var yubiKeyDeviceListener = YubiKeyDeviceListener.Instance;
                // Use a stopwatch to make sure we wont get stuck in an infinite loop.
                Stopwatch stopwatch = new Stopwatch();

                // Register event handlers for remove and (re)insert
                yubiKeyDeviceListener.Removed += YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived += YubiKeyArrived;


                // Wait for the YubiKey to be removed
                // If the YubiKey is not removed within 10 seconds, the reset will be aborted
                stopwatch.Start();
                while (!_yubiKeyRemoved)
                {
                    System.Threading.Thread.Sleep(100); // Prevent CPU overuse while waiting
                    if (stopwatch.Elapsed.TotalSeconds > 10)
                    {
                        throw new Exception("YubiKey was not removed within 10 seconds. Reset aborted.");
                    }
                }

                // Wait for the YubiKey to be reinserted
                // If the YubiKey is not removed within 10 seconds, the reset will be aborted
                stopwatch.Restart();
                while (!_yubiKeyArrived)
                {
                    System.Threading.Thread.Sleep(100); // Prevent CPU overuse while waiting
                    if (stopwatch.Elapsed.TotalSeconds > 10)
                    {
                        throw new Exception("YubiKey was not inserted within 10 seconds. Reset aborted.");
                    }
                }

                // Unregister the event handler after reset is completed
                // is this needed, we are disposing the listener?
                yubiKeyDeviceListener.Removed -= YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived -= YubiKeyArrived;

                // Proceed with the reset after the YubiKey is (re)inserted
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
                            // This should not happen anymore after the forcing of yubikey reinsertion above.
                            throw new Exception("Failed to reset, YubiKey needs to be reinserted within 5 seconds.");

                        case ResponseStatus.Success:
                            break;
                    }

                    YubiKeyModule._fido2PIN = null;
                    WriteInformation("YubiKey FIDO applet successfully reset.", new string[] { "FIDO2", "Info" });
                }
            }
        }

        private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (YubiKeyModule._yubikey!.SerialNumber == e.Device.SerialNumber)
            {
                _yubiKeyRemoved = true;
            }
        }

        private void YubiKeyArrived(object? sender, YubiKeyDeviceEventArgs e)
        {
            if (YubiKeyModule._yubikey!.SerialNumber == e.Device.SerialNumber)
            {
                _yubiKeyArrived = true;
            }
        }
    }
}