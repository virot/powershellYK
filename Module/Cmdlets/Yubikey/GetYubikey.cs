using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using powershellYK.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Get, "Yubikey")]
    public class GetYubikeyCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                var yubiKeys = YubiKeyDevice.FindAll();
                if (yubiKeys.Count() == 1)
                {
                    YubiKeyModule._yubikey = (YubiKeyDevice)yubiKeys.First();
                }
                else
                {
                    new Exception("None or multiple YubiKeys found");
                }
            }
            else
            {
                var yubiKey = YubiKeyDevice.FindAll().Where(yk => yk.SerialNumber == YubiKeyModule._yubikey.SerialNumber).First();
                if (yubiKey is not null)
                {
                    YubiKeyModule._yubikey = (YubiKeyDevice)yubiKey;
                }
            }
            WriteObject(new YubikeyInformation(YubiKeyModule._yubikey!));
        }
    }
}