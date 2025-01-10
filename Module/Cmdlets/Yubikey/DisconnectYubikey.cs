using powershellYK.support;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Disconnect, "YubiKey")]
    public class DisconnectYubikeyCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is not null)
            {
                if (YubiKeyModule._yubikey.SerialNumber is not null)
                {
                    WriteInformation($"Disconnected from {PowershellYKText.FriendlyName(YubiKeyModule._yubikey)} with serial: {YubiKeyModule._yubikey.SerialNumber}.", new string[] { "YubiKey" });
                }
                else
                {
                    WriteInformation($"Disonnected from {PowershellYKText.FriendlyName(YubiKeyModule._yubikey)} with serial: N/A.", new string[] { "YubiKey" });
                }
                YubiKeyModule._yubikey = null;
                YubiKeyModule.clearPassword();
            }
        }
    }
}