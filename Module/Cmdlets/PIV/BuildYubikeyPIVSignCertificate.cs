/// <summary>
/// Builds a signed certificate using a YubiKey PIV key.
/// Supports various certificate extensions and signing options.
/// Requires a YubiKey with PIV support and a valid signing certificate.
/// 
/// .EXAMPLE
/// Build-YubiKeyPIVSignCertificate -CertificateRequest $csr -Slot "Digital Signature"
/// Signs a certificate request using the Digital Signature slot
/// 
/// .EXAMPLE
/// Build-YubiKeyPIVSignCertificate -CertificateRequest $csr -Slot "PIV Authentication" -OutFile "signed.cer"
/// Signs a certificate request and saves it to a file
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;
using powershellYK.PIV;
using powershellYK.support.validators;
using Yubico.YubiKey.Cryptography;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Build, "YubiKeyPIVSignCertificate")]
    public class BuildYubikeySignedCertificateCommand : Cmdlet
    {
        // Parameter for the certificate request to be signed
        [TransformCertificateRequest_Path()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate request")]
        public PSObject? CertificateRequest { get; set; }

        // Parameter for the PIV slot to use for signing
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to sign certificate with")]
        public PIVSlot Slot { get; set; }

        // Parameter for the hash algorithm to use
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        // Parameter for output file
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

        // Parameter to output PEM encoding
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

        // Parameter for custom SKI (for debugging)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Custom SKI for debugging", DontShow = true)]
        public string? SKI { get; set; } = null;

        // Parameter for subject name override
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]
        public string? Subjectname { get; set; }

        // Parameter for certificate validity start
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Certificate to be valid from")]
        public DateTimeOffset NotBefore { get; set; } = DateTimeOffset.Now;

        // Parameter for certificate validity end
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Certificate to be valid until")]
        public DateTimeOffset NotAfter { get; set; } = DateTimeOffset.Now.AddYears(10);

        // Parameter for certificate serial number
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Serial number for certificate")]
        public byte[] SerialNumber { get; set; } = new byte[] { 112, 111, 119, 101, 114, 115, 104, 101, 108, 108, 89, 75 };

        // Parameter to make this a CA certificate
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Make this a CA certificate")]
        public SwitchParameter CertificateAuthority { get; set; }

        // Parameter for Subject Alternative Names
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "SubjectAlternativeNames for the certificate")]
        public string[] SubjectAltName { get; set; } = new string[] { };

        // Parameter for key usage flags
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Key usage options to include")]
        public X509KeyUsageFlags KeyUsage { get; set; } = X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment;

        // Parameter for AIA URL
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "AIA URL to include in signed certificates")]
        public string? AIAUrl { get; set; }

        private CertificateRequest? _request;

        // Called when the cmdlet begins processing
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    // Attempt to connect to a YubiKey
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

        // Main logic for signing the certificate
        protected override void ProcessRecord()
        {
            // Open a session with the YubiKey PIV application
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                X509SignatureGenerator signer;
                X509Certificate2 certificateCA;
                IPublicKey? publicKey;

                try
                {
                    // Retrieve the public key and CA certificate from the specified slot
                    publicKey = pivSession.GetMetadata(Slot).PublicKeyParameters;
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

                // If a subject name override is provided, create a new CertificateRequest
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

                // Add certificate extensions
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

                // Add Subject Key Identifier (SKI)
                if (SKI is not null)
                {
                    _request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(SKI, false));
                }
                else
                {
                    _request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(_request.PublicKey, false));
                }

                // Add Authority Key Identifier (AKI)
                WriteDebug("Extract the SKI of the certificateCA");
                X509AuthorityKeyIdentifierExtension AKI = X509AuthorityKeyIdentifierExtension.CreateFromCertificate(certificateCA, true, false);
                _request.CertificateExtensions.Add(AKI);

                // Add Authority Information Access (AIA) if provided
                if (AIAUrl is not null)
                {
                    IEnumerable<String> AIAstring = new List<string> { AIAUrl };
                    _request.CertificateExtensions.Add(new X509AuthorityInformationAccessExtension(null, AIAstring, false));
                }

                // Add Subject Alternative Names (SAN) if provided
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

                // Select the appropriate signature generator based on key type
                if (publicKey is RSAPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else if (publicKey is ECPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }
                else if (publicKey is Curve25519PublicKey)
                {
                    WriteError(new ErrorRecord(null, "UnknownAlgorithmFamily", ErrorCategory.OperationStopped, null));
                    return;
                }
                else
                {
                    throw new Exception("Unknown public Key algorithm");
                }

                // Create and sign the certificate
                X509Certificate2 signedCertificate = _request.Create(certificateCA.SubjectName, signer, NotBefore, NotAfter, SerialNumber);

                // Export the signed certificate as PEM
                string pemData = PemEncoding.WriteString("CERTIFICATE", signedCertificate!.Export(X509ContentType.Cert));

                // Output or save the certificate
                if (OutFile is not null)
                {
                    WriteCommandDetail($"Writing certificate to {OutFile.FullName}");
                    using (FileStream stream = OutFile.OpenWrite())
                    {
                        byte[] pemDataArray = System.Text.Encoding.UTF8.GetBytes(pemData);
                        stream.Write(pemDataArray, 0, pemDataArray.Length);
                    }
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

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
