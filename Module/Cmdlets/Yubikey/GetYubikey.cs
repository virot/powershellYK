using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace VirotYubikey.Cmdlets.Yubikey
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
            WriteObject(YubiKeyModule._yubikey);
        }
    }
}