using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubikey_Powershell;

namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommunications.Connect, "Yubikey")]
    public class ConnectYubikeyCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, HelpMessage = "Which yubikey to connect to")]
        public YubiKeyDevice? YubiKey { get; set; }

        protected override void BeginProcessing()
        {
            WriteDebug("ProcessRecord in Connect-Yubikey");
            if (YubiKey is not null)
            {
                YubiKeyModule._yubikey = YubiKey;
            } else {
                var yubiKeys = YubiKeyDevice.FindAll();
                //return the yubikey or an an array of yubikeys, if no youbikey is found throw [eu.virot.yubikey.nonfound            //return the yubikey or an an array of yubikeys, if no youbikey is found throw [eu.virot.yubikey.nonfound]

                if (yubiKeys.Count() == 1)
                {
                    YubiKeyModule._yubikey = (YubiKeyDevice)yubiKeys.First();
                }
                else
                {
                    new Exception("None or multiple YubiKeys found");
                }
            }


        }
    }
}