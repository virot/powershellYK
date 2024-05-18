using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using Yubikey_Powershell.support;


namespace Yubikey_Powershell.Cmdlets.PIV
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
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Save CSR as file")]

        public string? OutFile { get; set; } = null;

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
                publicKey = YubiKeyModule._pivSession.GetMetadata(Slot).PublicKey;
                if (publicKey is null)
                {
                    throw new Exception("Public key is null");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get public key for slot 0x{Slot.ToString("X2")}, does there exist a key?", e);
            }

            if (publicKey is PivRsaPublicKey)
            {
                PivRsaPublicKey pivRsaPublicKey = (PivRsaPublicKey)publicKey;
                RSA? rsaPublicKeyObject = null;
                var rsaParams = new RSAParameters
                {
                    Modulus = pivRsaPublicKey.Modulus.ToArray(),
                    Exponent = pivRsaPublicKey.PublicExponent.ToArray()
                };
                rsaPublicKeyObject = RSA.Create(rsaParams);
                CertificateRequest request = new CertificateRequest(Subjectname, rsaPublicKeyObject, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
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
                //public byte[] CreateSigningRequest(X509SignatureGenerator signatureGenerator);
                var signer = new YubiKeySignatureGenerator(YubiKeyModule._pivSession, Slot, rsaPublicKeyObject, RSASignaturePadding.Pss);
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