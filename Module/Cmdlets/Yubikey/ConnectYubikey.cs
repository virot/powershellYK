using powershellYK.Cmdlets.Other;
using System.Diagnostics;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Sample.PivSampleCode;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Connect, "Yubikey", DefaultParameterSetName = "Connect single Yubikey")]
    public class ConnectYubikeyCommand : PSCmdlet
    {

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, HelpMessage = "Which YubiKey to connect to", ParameterSetName = "Connect provided Yubikey")]
        public YubiKeyDevice? YubiKey { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Connect to YubiKey with Serial Number", ParameterSetName = "Connect Yubikey with Serialnumber")]
        public int? Serialnumber { get; set; }

        private YubiKeyDevice? _yubikey;

        protected override void BeginProcessing()
        {
        }
        protected override void ProcessRecord()
        {
            //Start with disconnecting the old yubikey if connected.
            if (YubiKeyModule._yubikey is not null)
            {
                WriteDebug("Disconnecting from previous YubiKey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Disconnect-Yubikey");
                myPowersShellInstance.Invoke();
            }

            WriteDebug("Starting the real connect part");
            switch (ParameterSetName)
            {
                case "Connect provided Yubikey":
                    _yubikey = YubiKey;
                    break;
                case "Connect single Yubikey":
                    var yubikeys = YubiKeyDevice.FindAll();
                    if (yubikeys.Count() == 1)
                    {
                       _yubikey = (YubiKeyDevice)yubikeys.First();
                        WriteDebug($"Found only one device, using {_yubikey.SerialNumber}");
                    }
                    break;
                case "Connect Yubikey with Serialnumber":
                    WriteDebug($"Looking for YubiKey with serial: {Serialnumber}");
                    _yubikey = (YubiKeyDevice)YubiKeyDevice.FindAll().Where(x => x.SerialNumber == Serialnumber).First();
                    break;
                default:
                    throw new Exception("Invalid ParameterSetName");
            }
            if (_yubikey is not null)
            {
                YubiKeyModule._yubikey = _yubikey;
            }
            else
            {
                throw new Exception("None or multiple YubiKeys found, Use Connect-Yubikey to specify which Yubikey to use");
            }
        }
    }
}