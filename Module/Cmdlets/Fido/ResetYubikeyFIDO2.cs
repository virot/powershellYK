using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Fido2.Commands;
using powershellYK.Exceptions;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyFIDO2", SupportsShouldProcess = true)]
    public class ResetYubikeyFIDO2Cmdlet : Cmdlet
    {
        private bool _yubiKeyRemoved = false;
        private bool _yubiKeyArrived = false;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug("Successfully connected");
            }

            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        protected override void ProcessRecord()
        {
                if (ShouldProcess("YubiKey FIDO2", "Reset"))
            {
                // Custom reset confirmation message
                if (ShouldContinue("This will delete all FIDO credentials, including FIDO U2F credentials, and restore factory settings. Proceed?", "WARNING!"))
                    Console.WriteLine("Remove and re-insert the YubiKey to perform the reset...");

                // Set up the YubiKeyDeviceListener
                var yubiKeyDeviceListener = YubiKeyDeviceListener.Instance;

                // Register event handlers for remove and (re)insert
                yubiKeyDeviceListener.Removed += YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived += YubiKeyArrived;

                // Wait for the YubiKey to be removed
                while (!_yubiKeyRemoved)
                {
                    System.Threading.Thread.Sleep(100); // Prevent CPU overuse while waiting
                }

                //Console.WriteLine("Reinsert the YubiKey...");

                // Wait for the YubiKey to be reinserted
                while (!_yubiKeyArrived)
                {
                    System.Threading.Thread.Sleep(100); // Prevent CPU overuse while waiting
                }

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
                            throw new Exception("Failed to reset, YubiKey needs to be reinserted within 5 seconds.");

                        case ResponseStatus.Success:
                            break;
                    }
                    
                    YubiKeyModule._fido2PIN = null;
                }

                // Unregister the event handler after reset is completed
                yubiKeyDeviceListener.Removed -= YubiKeyRemoved;
                yubiKeyDeviceListener.Arrived -= YubiKeyArrived;
            }
        }

        private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs e)
        {
            _yubiKeyRemoved = true;
        }

        private void YubiKeyArrived(object? sender, YubiKeyDeviceEventArgs e)
        {
            _yubiKeyArrived = true;
        }
    }
}