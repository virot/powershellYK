using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;
using powershellYK.PIV;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Build, "YubiKeyPIVSignCertificate")]
    public class BuildYubikeySignedCertificateCommand : Cmdlet
    {
        [TransformCertificateRequest_Path()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate request")]
        public PSObject? CertificateRequest { get; set; }
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to sign certificate with")]
        public PIVSlot Slot { get; set; }
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Custom SKI for debugging", DontShow = true)]
        public string? SKI { get; set; } = null;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]
        public string? Subjectname { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Certificate to be valid from")]
        public DateTimeOffset NotBefore { get; set; } = DateTimeOffset.Now;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Certificate to be valid until")]
        public DateTimeOffset NotAfter { get; set; } = DateTimeOffset.Now.AddYears(10);
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Serial number for certificate")]
        public byte[] SerialNumber { get; set; } = new byte[] { 112, 111, 119, 101, 114, 115, 104, 101, 108, 108, 89, 75 };
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Make this a CA certificate")]
        public SwitchParameter CertificateAuthority { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "SubjectAlternativeNames for the certificate")]
        public string[] SubjectAltName { get; set; } = new string[] { };
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Key usage options to include")]
        public X509KeyUsageFlags KeyUsage { get; set; } = X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "AIA URL to include in signed certificates")]
        public string? AIAUrl { get; set; }


        private CertificateRequest? _request;

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

                X509SignatureGenerator signer;
                X509Certificate2 certificateCA;
                PivPublicKey publicKey;

                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    certificateCA = pivSession.GetCertificate(Slot);
                    if (certificateCA.PublicKey is null)
                    {
                        throw new Exception($"No certificate that can sign in slot {Slot}, does there exist a certificate?");
                    }
                }
                catch
                {
                    throw new Exception($"No certificate that can sign in slot {Slot}, does there exist a certificate?");
                }


                // If we want to replace the SubjectName we need to create a new CertificateReqest and sign that one instead.

                if (Subjectname is null)
                {
                    _request = (CertificateRequest)CertificateRequest!.BaseObject;
                }
                else
                {
                    if (((CertificateRequest)CertificateRequest!.BaseObject).PublicKey.Oid.FriendlyName == "RSA")
                    {
                        _request = new CertificateRequest(Subjectname, ((CertificateRequest)CertificateRequest!.BaseObject).PublicKey.GetRSAPublicKey()!, HashAlgorithm, RSASignaturePadding.Pkcs1);
                    }
                    else
                    {
                        _request = new CertificateRequest(Subjectname, ((CertificateRequest)CertificateRequest!.BaseObject).PublicKey.GetECDsaPublicKey()!, HashAlgorithm);
                    }
                }

                if (CertificateAuthority.IsPresent)
                {
                    _request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
                }
                else
                {
                    _request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
                    _request.CertificateExtensions.Add(new X509KeyUsageExtension(KeyUsage, true));
                    _request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2"), new Oid("1.3.6.1.4.1.311.20.2.2") }, false));
                }

                // Add SKI
                if (SKI is not null)
                {
                    _request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(SKI, false));
                }
                else
                {
                    _request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(_request.PublicKey, false));
                }

                // Add AKI
                WriteDebug("Extract the SKI of the certificateCA");
                X509AuthorityKeyIdentifierExtension AKI = X509AuthorityKeyIdentifierExtension.CreateFromCertificate(certificateCA, true, false);
                _request.CertificateExtensions.Add(AKI);

                // Add AIA link if supplied
                if (AIAUrl is not null)
                {
                    IEnumerable<String> AIAstring = new List<string> { AIAUrl };
                    _request.CertificateExtensions.Add(new X509AuthorityInformationAccessExtension(null, AIAstring, false));
                }

                // Add SAN / SubjectAltName
                if (SubjectAltName.Length > 0)
                {
                    var sanBuilder = new SubjectAlternativeNameBuilder();
                    foreach (var san in SubjectAltName)
                    {
                        WriteDebug($"Parsing SAN: {san}");
                        int colonIndex = san.IndexOf(' ');
                        if (san.StartsWith("DNS", StringComparison.CurrentCultureIgnoreCase))
                        {
                            sanBuilder.AddDnsName(san.Substring(colonIndex + 1).Trim());
                        }
                        else if (san.StartsWith("MAIL", StringComparison.CurrentCultureIgnoreCase))
                        {
                            sanBuilder.AddEmailAddress(san.Substring(colonIndex + 1).Trim());
                        }
                        else if (san.StartsWith("UPN", StringComparison.CurrentCultureIgnoreCase))
                        {
                            sanBuilder.AddUserPrincipalName(san.Substring(colonIndex + 1).Trim());
                        }
                    }
                    _request.CertificateExtensions.Add(sanBuilder.Build());
                }

                if (publicKey is PivRsaPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }

                X509Certificate2 signedCertificate = _request.Create(certificateCA.SubjectName, signer, NotBefore, NotAfter, SerialNumber);

                string pemData = PemEncoding.WriteString("CERTIFICATE", signedCertificate!.Export(X509ContentType.Cert));

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
                        WriteObject(signedCertificate);
                    }
                }
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}
