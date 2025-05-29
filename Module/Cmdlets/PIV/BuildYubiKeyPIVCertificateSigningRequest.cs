/// <summary>
/// Creates a Certificate Signing Request (CSR) for a key in a specified YubiKey PIV slot.
/// Supports RSA and ECC keys, with optional attestation certificate inclusion.
/// Requires a YubiKey with PIV support and firmware version 5.3.0 or newer.
/// 
/// .EXAMPLE
/// Build-YubiKeyPIVCertificateSigningRequest -Slot "PIV Authentication" -Subjectname "CN=Test User"
/// Creates a CSR for the PIV Authentication slot
/// 
/// .EXAMPLE
/// Build-YubiKeyPIVCertificateSigningRequest -Slot "Digital Signature" -Attestation -OutFile "request.csr"
/// Creates a CSR with attestation and saves it to a file
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.PIV;
using powershellYK.support.transform;
using powershellYK.support.validators;
using Yubico.YubiKey.Cryptography;

namespace powershellYK.Cmdlets.PIV
{
    [Alias("New-YubikeyPIVCSR")]
    [Cmdlet(VerbsLifecycle.Build, "YubiKeyPIVCertificateSigningRequest")]
    public class BuildYubiKeyPIVCertificateSigningRequestCmdlet : Cmdlet
    {
        // Parameter for the PIV slot to create CSR for
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot", ParameterSetName = "With Attestation")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot", ParameterSetName = "Without Attestation")]
        public PIVSlot Slot { get; set; }

        // Parameter to include attestation certificate in the CSR
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Include attestation certificate in CSR", ParameterSetName = "With Attestation")]
        public SwitchParameter Attestation { get; set; }

        // Parameter to specify where to store attestation in the CSR
        [ValidateSet("Both", "Legacy", "Standard", ErrorMessage = null)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "OID to store attestation in CSR.", ParameterSetName = "With Attestation")]
        public string AttestationLocation { get; set; } = "Both";

        // Parameter for the subject name of the certificate
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]
        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";

        // Parameter for the output file path
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Save CSR as file")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

        // Parameter for the hash algorithm to use
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        // Parameter to output CSR in PEM format
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

        // Called when the cmdlet begins processing
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
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

        // Main logic for creating the CSR
        protected override void ProcessRecord()
        {
            // Open a session with the YubiKey PIV application
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN entry
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                CertificateRequest request;
                X509SignatureGenerator signer;

                // Verify firmware version requirement
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).FirmwareVersion < new FirmwareVersion(5, 3, 0))
                {
                    throw new NotSupportedException("This feature requires firmware version 5.3.0 or newer.");
                }

                // Get metadata and public key for the slot
                PivMetadata? metadata = null;
                IPublicKey? publicKey = null;
                //IPublicKey? publicKeyParam = null;
                try
                {
                    metadata = pivSession.GetMetadata(Slot);
                    publicKey = metadata.PublicKeyParameters;
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get metadata for slot {Slot}.", e);
                }

                // Verify public key exists
                if (publicKey is null)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does there exist a key?");
                }

                // Verify key was generated on YubiKey if attestation is requested
                if (Attestation.IsPresent)
                {
                    if (metadata.KeyStatus != PivKeyStatus.Generated)
                    {
                        throw new InvalidOperationException($"Private key must be generated on YubiKey for attested certificate requests. {Slot} is {metadata.KeyStatus}.");
                    }
                }

                // Create certificate request based on key type
                if (publicKey is RSAPublicKey)
                {
                    // Create RSA certificate request
                    using (RSA rsa = RSA.Create())
                    {
                        rsa.ImportSubjectPublicKeyInfo(publicKey.ExportSubjectPublicKeyInfo(), out _);
                        request = new CertificateRequest(Subjectname, rsa, HashAlgorithm, RSASignaturePadding.Pkcs1);
                    }
                }
                else if (publicKey is ECPublicKey)
                {
                    // Set hash algorithm based on ECC key size
                    HashAlgorithm = publicKey.KeyType switch
                    {
                        KeyType.ECP256 => HashAlgorithmName.SHA256,
                        KeyType.ECP384 => HashAlgorithmName.SHA384,
                        KeyType.ECP521 => HashAlgorithmName.SHA512,
                        _ => throw new Exception("Unknown Public key algorithm")
                    };

                    // Create ECC certificate request
                    using (ECDsa ecc = ECDsa.Create())
                    {
                        ecc.ImportSubjectPublicKeyInfo(publicKey.ExportSubjectPublicKeyInfo(), out _);
                        WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");
                        request = new CertificateRequest(Subjectname, ecc, HashAlgorithm);
                    }
                }
                else if (publicKey is Curve25519PublicKey)
                {
                    // If needed otherwise use the default SHA256
                    throw new NotSupportedException("Curve25519 is not supported for CSR generation.");
                }
                else
                {
                    throw new Exception($"Unknown public key type {publicKey.KeyType}");
                }

                // Add attestation certificates if requested
                if (Attestation.IsPresent)
                {
                    // Get attestation certificates
                    X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
                    byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                    X509Certificate2 yubikeyIntermediateAttestationCertificate = pivSession.GetAttestationCertificate();
                    byte[] yubikeyIntermediateAttestationCertificateBytes = yubikeyIntermediateAttestationCertificate.Export(X509ContentType.Cert);

                    // Define OIDs for attestation extensions
                    Oid oidIntermediate = new Oid("1.3.6.1.4.1.41482.3.2");
                    Oid oidSlotAttestationStandard = new Oid("1.3.6.1.4.1.41482.3.1");
                    Oid oidSlotAttestationLegacy = new Oid("1.3.6.1.4.1.41482.3.11");

                    // Add attestation extensions based on location preference
                    switch (AttestationLocation)
                    {
                        case "Both":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationStandard, slotAttestationCertificateBytes, false));
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationLegacy, slotAttestationCertificateBytes, false));
                            break;
                        case "Standard":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationStandard, slotAttestationCertificateBytes, false));
                            break;
                        case "Legacy":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationLegacy, slotAttestationCertificateBytes, false));
                            break;
                    }
                    request.CertificateExtensions.Add(new X509Extension(oidIntermediate, yubikeyIntermediateAttestationCertificateBytes, false));
                }

                // Create appropriate signature generator based on key type
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
                    //signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                    throw new NotSupportedException("Curve25519 is not supported for CSR generation.");
                }
                else
                {
                    throw new Exception($"Unknown public key type {publicKey.KeyType}");
                }

                // Sign the CSR and convert to PEM format
                byte[] requestSigned = request.CreateSigningRequest(signer);
                string pemData = PemEncoding.WriteString("CERTIFICATE REQUEST", requestSigned);

                // Handle output based on parameters
                if (OutFile is not null)
                {
                    // Write CSR to file
                    WriteCommandDetail($"Writing certificate to {OutFile.FullName}");
                    using (FileStream stream = OutFile.OpenWrite())
                    {
                        byte[] pemDataArray = System.Text.Encoding.UTF8.GetBytes(pemData);
                        stream.Write(pemDataArray, 0, pemDataArray.Length);
                    }
                }
                else
                {
                    // Output to pipeline
                    if (PEMEncoded.IsPresent)
                    {
                        // Output as PEM string
                        WriteObject(pemData);
                    }
                    else
                    {
                        // Output as certificate request object
                        var csrObject = CertificateRequest.LoadSigningRequestPem(pemData.AsSpan(), HashAlgorithm, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                        WriteObject(csrObject);
                    }
                }
                WriteDebug("ProcessRecord in New-YubikeyPIVCSR");
            }
        }

        // Called when the cmdlet ends processing
        protected override void EndProcessing()
        {
        }
    }
}
