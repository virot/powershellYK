using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace Yubikey_Powershell.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Disconnect, "Yubikey")]
    public class DisconnectYubikeyCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            YubiKeyModule._yubikey = null;
        }
    }
}