using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Disconnect, "Yubikey")]
    public class DisconnectYubikeyCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            YubiKeyModule._yubikey = null;
            YubiKeyModule.clearPassword();
            WriteInformation("Disconnected from YubiKey.", new string[] { "YubiKey" });
        }
    }
}