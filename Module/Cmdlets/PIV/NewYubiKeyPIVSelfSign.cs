using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.PIV;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubiKeyPIVSelfSign", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVSelfSignCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Sign a self-signed certificate for slot")]
        public PIVSlot Slot { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]
        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Hash algoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
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
                X500DistinguishedName dn;
                X509SubjectKeyIdentifierExtension certificateSKI;

                PivPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    if (publicKey is null)
                    {
                        throw new Exception("Public key is null!");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does the key exist?", e);
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
                        _ => throw new Exception("Unknown public Key algorithm")
                    };
                    WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");
                    request = new CertificateRequest(Subjectname, (ECDsa)dotNetPublicKey, HashAlgorithm);
                }

                X509BasicConstraintsExtension x509BasicConstraintsExtension = new X509BasicConstraintsExtension(true, true, 2, true);
                X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, false);
                request.CertificateExtensions.Add(x509BasicConstraintsExtension);
                request.CertificateExtensions.Add(x509KeyUsageExtension);

                // Add SKI
                certificateSKI = new X509SubjectKeyIdentifierExtension(new PublicKey(dotNetPublicKey), false);
                request.CertificateExtensions.Add(certificateSKI);

                DateTimeOffset notBefore = DateTimeOffset.Now;
                DateTimeOffset notAfter = notBefore.AddYears(10);
                byte[] serialNumber = new byte[] { 0x01 };

                if (publicKey is PivRsaPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }

                try
                {
                    dn = new X500DistinguishedName(Subjectname);
                }
                catch (Exception e) { throw new Exception("Failed to create X500DistinguishedName", e); }
                X509Certificate2 selfCert = request.Create(dn, signer, notBefore, notAfter, serialNumber);

                bool certExists = false;
                try
                {
                    X509Certificate2 certtest = pivSession.GetCertificate(Slot);
                    certExists = true;
                }
                catch { }

                if (!certExists || ShouldProcess($"Certificate in slot {Slot}", "New"))
                {
                    WriteDebug($"Importing created certificate into YubiKey slot {Slot}");
                    pivSession.ImportCertificate(Slot, selfCert);
                }
                WriteDebug("ProcessRecord in New-YubikeyPIVSelfSign");
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}
