using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using powershellYK.support;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Build, "YubikeySignedCertificate")]
    public class BuildYubikeySignedCertificateCommand : Cmdlet
    {
        [TransformCertificateRequest_Path()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate request")]
        public PSObject? CertificateRequest { get; set; }
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to sign certificate with")]
        public byte Slot { get; set; }
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        private CertificateRequest? _request;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
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

                X509SignatureGenerator signer;
                X509Certificate2 certificate;
                PivPublicKey publicKey;

                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    certificate = pivSession.GetCertificate(Slot);
                    if (certificate.PublicKey is null)
                    {
                        throw new Exception($"No certificate that can sign in slot 0x{Slot.ToString("X2")}, does there exist a certificate?");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Unable to retrive certificate to sign");
                }

                _request = (CertificateRequest)CertificateRequest!.BaseObject;
                X509BasicConstraintsExtension x509BasicConstraintsExtension = new X509BasicConstraintsExtension(false, true, 0, true);
                X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension((X509KeyUsageFlags.DigitalSignature), false);
                X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2") }, false);
                _request.CertificateExtensions.Add(x509BasicConstraintsExtension);
                _request.CertificateExtensions.Add(x509KeyUsageExtension);
                _request.CertificateExtensions.Add(x509EnhancedKeyUsageExtension);


                // Add SKI
                X509SubjectKeyIdentifierExtension certificateSKI = new X509SubjectKeyIdentifierExtension(_request.PublicKey, true);
                _request.CertificateExtensions.Add(certificateSKI);

                DateTimeOffset notBefore = DateTimeOffset.Now;
                DateTimeOffset notAfter = notBefore.AddYears(10);
                byte[] serialNumber = new byte[] { 112, 111, 119, 101, 114, 115, 104, 101, 108, 108, 89, 75 };

                if (publicKey is PivRsaPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }

                X509Certificate2 signedCertificate = _request.Create(certificate.SubjectName, signer, notBefore, notAfter, serialNumber);

                WriteObject(signedCertificate);
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}