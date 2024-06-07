using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Connect, "Yubikey")]
    public class ConnectYubikeyCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, HelpMessage = "Which yubikey to connect to")]
        public YubiKeyDevice? YubiKey { get; set; }

        protected override void BeginProcessing()
        {
            //Start with disconnecting the old yubikey if connected.
            if (YubiKeyModule._yubikey is not null)
            {
                DisconnectYubikeyCommand disconnectYubikey = new DisconnectYubikeyCommand();
                disconnectYubikey.Invoke();
            }
            WriteDebug("ProcessRecord in Connect-Yubikey");
            if (YubiKey is not null)
            {
                WriteDebug("Using Yubikey from argument");
                YubiKeyModule._yubikey = YubiKey;
            }
            else
            {
                WriteDebug("Searching for yubikeys");
                var yubiKeys = YubiKeyDevice.FindAll();

                if (yubiKeys.Count() == 1)
                {
                    YubiKeyModule._yubikey = (YubiKeyDevice)yubiKeys.First();
                }
                else
                {
                    WriteDebug("Multiple or No Yubikeys found");
                    throw new Exception("None or multiple YubiKeys found");
                }
            }



        }
    }
}