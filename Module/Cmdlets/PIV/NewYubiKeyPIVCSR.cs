using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using VirotYubikey.support;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVCSR")]
    public class NewYubiKeyPIVCSRCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot")]

        public byte Slot { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Include attestion certificate in CSR")]

        public SwitchParameter Attestation { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subjectname of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Save CSR as file")]
        public string? OutFile { get; set; } = null;

        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        protected override void ProcessRecord()
        {
            CertificateRequest request;
            X509SignatureGenerator signer;

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



            using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);

            if (publicKey is PivRsaPublicKey)
            {
                request = new CertificateRequest(Subjectname, (RSA)dotNetPublicKey, HashAlgorithm, RSASignaturePadding.Pkcs1);
            }
            else
            {
                HashAlgorithm = publicKey.Algorithm switch
                {
                    PivAlgorithm.EccP256 => HashAlgorithmName.SHA256,
                    PivAlgorithm.EccP384 => HashAlgorithmName.SHA384,
                    _ => throw new Exception("Unknown PublicKey algorithm")
                };
                WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");
                request = new CertificateRequest(Subjectname, (ECDsa)dotNetPublicKey, HashAlgorithm);
            }

            if (Attestation.IsPresent)
            {
                X509Certificate2 slotAttestationCertificate = YubiKeyModule._pivSession.CreateAttestationStatement(Slot);
                byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                X509Certificate2 yubikeyIntermediateAttestationCertificate = YubiKeyModule._pivSession.GetAttestationCertificate();
                byte[] yubikeyIntermediateAttestationCertificateBytes = yubikeyIntermediateAttestationCertificate.Export(X509ContentType.Cert);
                Oid oidIntermediate = new Oid("1.3.6.1.4.1.41482.3.2");
                Oid oidSlotAttestation = new Oid("1.3.6.1.4.1.41482.3.11");
                request.CertificateExtensions.Add(new X509Extension(oidSlotAttestation, slotAttestationCertificateBytes, false));
                request.CertificateExtensions.Add(new X509Extension(oidIntermediate, yubikeyIntermediateAttestationCertificateBytes, false));
            }

            if (publicKey is PivRsaPublicKey)
            {
                signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
            }
            else
            {
                signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, publicKey);
            }

            byte[] requestSigned = request.CreateSigningRequest(signer);
            string pemData = PemEncoding.WriteString("CERTIFICATE REQUEST", requestSigned);
            if (OutFile is not null)
            {
                File.WriteAllText(OutFile, pemData);
            }
            else
            {
                WriteObject(pemData);
            }
            WriteDebug("ProcessRecord in New-YubikeyPIVCSR");
        }


        protected override void EndProcessing()
        {
        }
    }
}