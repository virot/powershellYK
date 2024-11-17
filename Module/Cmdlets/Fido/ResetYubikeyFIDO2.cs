using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Fido2.Commands;
using powershellYK.Exceptions;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyFIDO2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]

    public class ResetYubikeyFIDO2Cmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
            }
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator");
            }
        }

        protected override void ProcessRecord()
        {
            if (ShouldProcess("Yubikey FIDO2", "Reset"))
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    ResetCommand resetCommand = new Yubico.YubiKey.Fido2.Commands.ResetCommand();
                    WriteWarning("Please touch the Yubikey to allow the reset.");
                    ResetResponse reply = fido2Session.Connection.SendCommand(resetCommand);
                    switch (reply.Status)
                    {
                        default:
                            throw new Exception("Unknown status. (Update ResetYubikeyFIDO2Cmdlet)");

                        case ResponseStatus.Failed:
                            throw new Exception("Please touch the Yubikey to allow the reset.");

                        case ResponseStatus.ConditionsNotSatisfied:
                            throw new Exception($"Failed to reset, Yubikey needs to be inserted within the last 5 seconds.");

                        case ResponseStatus.Success:
                            break;
                    }
                    WriteWarning("Restart FIDO2 Application by removing and reinserting the Yubikey.");
                    YubiKeyModule._fido2PIN = null;
                }
            }
        }
    }
}