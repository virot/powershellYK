using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using VirotYubikey.support;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVSelfSign")]
    public class NewYubiKeyPIVSelfSignCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Sign a self signed cert for slot")]

        public byte Slot { get; set; }

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subjectname of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        protected override void ProcessRecord()
        {
            if (YubiKeyModule._pivSession is null)
            {
                //throw new Exception("PIV not connected, use Connect-YubikeyPIV first");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyPIV");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            PivPublicKey? publicKey = null;
            try
            {
                publicKey = YubiKeyModule._pivSession!.GetMetadata(Slot).PublicKey;
                if (publicKey is null)
                {
                    throw new Exception("Public key is null");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get public key for slot 0x{Slot.ToString("X2")}, does there exist a key?", e);
            }
            WriteDebug("ProcessRecord in New-YubikeyPIVSelfSign");
        }


        protected override void EndProcessing()
        {
        }
    }
}