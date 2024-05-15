using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubikey_Powershell.support;


namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVKey")]
    public class NewYubiKeyPIVKeyCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key for")]

        public byte Slot { get; set; }

        [ValidateSet("Rsa1024", "Rsa2048", "EccP256", "EccP384", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; }

        [ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;
        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "ManagementKey")]
        public string ManagementKey { get; set; } = "010203040506070801020304050607080102030405060708";


        protected override void ProcessRecord()
        {
            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]             //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]
            if (YubiKeyModule._connection is null) { throw new Exception("PIV not connected, use Connect-YubikeyPIV first"); }

            try
            {
                WriteDebug("ProcessRecord in New-YubikeyPIVKey");
                GenerateKeyPairCommand newKey = new GenerateKeyPairCommand(Slot, Algorithm, PinPolicy, TouchPolicy);
                GenerateKeyPairResponse response = YubiKeyModule._connection.SendCommand(newKey);
                response.GetData();
            }
            catch (Exception e)
            {
                throw new Exception("Could not create keypair", e);
            }

        }
    }
}