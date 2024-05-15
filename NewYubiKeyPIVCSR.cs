using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using Yubikey_Powershell.support;


namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVCSR")]
    public class NewYubiKeyPIVCSRCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot")]

        public byte Slot { get; set; }

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Include attestion certificate in CSR")]

        public SwitchParameter Attestation { get; set; }
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subjectname of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._pivSession is null) { throw new Exception("PIV not connected, use Connect-YubikeyPIV first"); }


        }

        protected override void ProcessRecord()
        {
            PivPublicKey publicKey = YubiKeyModule._pivSession.GetMetadata(Slot).PublicKey;
            if (publicKey is null)
            {
                throw new Exception("Public key is null");
            }

            if (publicKey is PivRsaPublicKey)
            {
                PivRsaPublicKey pivRsaPublicKey = (PivRsaPublicKey)publicKey;
                RSA rsaPublicKeyObject = null;
                var rsaParams = new RSAParameters
                        {
                            Modulus = pivRsaPublicKey.Modulus.ToArray(),
                            Exponent = pivRsaPublicKey.PublicExponent.ToArray()
                        };

                rsaPublicKeyObject = RSA.Create(rsaParams);
                CertificateRequest request = new CertificateRequest(Subjectname, rsaPublicKeyObject, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                //public byte[] CreateSigningRequest(X509SignatureGenerator signatureGenerator);
                var signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, rsaPublicKeyObject, RSASignaturePadding.Pss);
                byte[] requestSigned = request.CreateSigningRequest(signer);
                char[] pemData = PemEncoding.Write("CERTIFICATE REQUEST", requestSigned);
                WriteObject(pemData);
            }
            else
            {
                throw new Exception("Public key is not an RSA key");
            }
            WriteDebug("ProcessRecord in New-YubikeyPIVCSR");
        }


        protected override void EndProcessing()
        {
        }
    }
}