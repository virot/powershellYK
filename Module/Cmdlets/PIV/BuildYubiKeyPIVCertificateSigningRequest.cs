using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.PIV;


namespace powershellYK.Cmdlets.PIV
{
    [Alias("New-YubikeyPIVCSR")]
    [Cmdlet(VerbsLifecycle.Build, "YubiKeyPIVCertificateSigningRequest")]
    public class BuildYubiKeyPIVCertificateSigningRequestCmdlet : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot")]
        public PIVSlot Slot { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Include attestion certificate in CSR")]
        public SwitchParameter Attestation { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Save CSR as file")]
        public string? OutFile { get; set; } = null;

        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                CertificateRequest request;
                X509SignatureGenerator signer;


                PivPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    if (publicKey is null)
                    {
                        throw new Exception("Public key is null");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does there exist a key?", e);
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
                    X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
                    byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                    X509Certificate2 yubikeyIntermediateAttestationCertificate = pivSession.GetAttestationCertificate();
                    byte[] yubikeyIntermediateAttestationCertificateBytes = yubikeyIntermediateAttestationCertificate.Export(X509ContentType.Cert);
                    Oid oidIntermediate = new Oid("1.3.6.1.4.1.41482.3.2");
                    Oid oidSlotAttestation = new Oid("1.3.6.1.4.1.41482.3.11");
                    request.CertificateExtensions.Add(new X509Extension(oidSlotAttestation, slotAttestationCertificateBytes, false));
                    request.CertificateExtensions.Add(new X509Extension(oidIntermediate, yubikeyIntermediateAttestationCertificateBytes, false));
                }

                if (publicKey is PivRsaPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }

                byte[] requestSigned = request.CreateSigningRequest(signer);
                string pemData = PemEncoding.WriteString("CERTIFICATE REQUEST", requestSigned);
                if (OutFile is not null)
                {
                    File.WriteAllText(OutFile, pemData);
                }
                else
                {
                    if (PEMEncoded.IsPresent)
                    {
                        WriteObject(pemData);
                    }
                    else
                    {
                        WriteObject(requestSigned);
                    }
                }
                WriteDebug("ProcessRecord in New-YubikeyPIVCSR");
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}
