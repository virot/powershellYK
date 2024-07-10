using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Fido2.Commands;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyFIDO2")]

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
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                ResetCommand resetCommand = new Yubico.YubiKey.Fido2.Commands.ResetCommand();
                ResetResponse reply = fido2Session.Connection.SendCommand(resetCommand);
                if (reply.Status != ResponseStatus.Success)
                {
                    WriteWarning($"Failed to reset FIDO2 credentials: {reply.Status}, Yubikey needs to be inserted within the last 5 seconds.");
                }
            }
        }
    }
}