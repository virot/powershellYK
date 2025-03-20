// Imports
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using static System.Security.Cryptography.X509Certificates.CertificateRequest;
using powershellYK.PIV;
using System.Reflection;
using System.Text;

namespace powershellYK.Cmdlets.Other
{
    // Allows the cmdlet to be called by alias for backward compatibility
    [Alias("Confirm-YubiKeyAttestation")]
    [Cmdlet(VerbsLifecycle.Confirm, "YubiKeyPIVAttestation")]
    public class ConfirmYubikeyPIVAttestationCmdlet : PSCmdlet
    {
        #region Parameter Definitions
        // Parameters for handling Certificate Signing Requests (CSR)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithExternalAttestation-Object")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithBuiltinAttestation-Object")]
        public CertificateRequest? CertificateRequest { get; set; }

        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithExternalAttestation-File")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithBuiltinAttestation-File")]
        public System.IO.FileInfo? CertificateRequestFile { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "requestWithExternalAttestation-Object")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "JustAttestCertificate-Object")]
        public X509Certificate2? AttestationCertificate { get; set; }

        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "requestWithExternalAttestation-File")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "JustAttestCertificate-File")]
        public System.IO.FileInfo? AttestationCertificateFile { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "requestWithExternalAttestation-Object")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "JustAttestCertificate-Object")]
        public X509Certificate2? IntermediateCertificate { get; set; }

        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "requestWithExternalAttestation-File")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "JustAttestCertificate-File")]
        public System.IO.FileInfo? IntermediateCertificateFile { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CertificateIncludingAttestation", ParameterSetName = "CertificateIncludingAttestation-Object")]
        public X509Certificate2? CertificateIncludingAttestation { get; set; }

        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CertificateIncludingAttestation", ParameterSetName = "CertificateIncludingAttestation-File")]
        public System.IO.FileInfo? CertificateIncludingAttestationFile { get; set; }

        #endregion

        #region Private Fields
        // Internal storage for processed certificates and requests
        private CertificateRequest? _CertificateRequest;
        private X509Certificate2? _AttestationCertificate;
        private X509Certificate2? _IntermediateCertificate;
        private X509Certificate2? _CertificateIncludingAttestation;

        // Regex pattern for PEM format validation
        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        // Storage for extracted attestation data
        private uint? _out_SerialNumber;
        private FirmwareVersion? _out_FirmwareVersion;
        private PivPinPolicy? _out_PinPolicy;
        private PivTouchPolicy? _out_TouchPolicy;
        private FormFactor? _out_FormFactor;
        private PIVSlot _out_Slot;
        private bool? _out_isFIPSSeries = null;
        private bool? _out_isCSPNSeries = null;
        private bool? _out_AttestationMatchesCSR = null;
        private PivAlgorithm? _out_Algorithm;
        private string? _out_AttestationDataLocation = null;
        #endregion

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            // Initialize regex for PEM format parsing
            Regex regex = new Regex(pemFormatRegex, RegexOptions.Singleline);
            Match? match;
            X509Extension? extensioncsr;

            WriteDebug($"Cmdlet triggered with ParameterSetName={ParameterSetName}");

            #region Load Input Data
            // Load certificate request from object or file
            if (ParameterSetName == "requestWithBuiltinAttestation-Object" || ParameterSetName == "requestWithExternalAttestation-Object")
            {
                _CertificateRequest = CertificateRequest;
            }
            if (ParameterSetName == "requestWithBuiltinAttestation-File" || ParameterSetName == "requestWithExternalAttestation-File")
            {
                using (FileStream fileStream = CertificateRequestFile!.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string request = reader.ReadToEnd();
                        _CertificateRequest = LoadSigningRequestPem(request, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                    }
                }
            }
            if (ParameterSetName == "requestWithExternalAttestation-Object" || ParameterSetName == "JustAttestCertificate-Object")
            {
                _AttestationCertificate = AttestationCertificate;
                _IntermediateCertificate = IntermediateCertificate;
            }
            if (ParameterSetName == "requestWithExternalAttestation-File" || ParameterSetName == "JustAttestCertificate-File")
            {
                WriteDebug($"Reading Attestation certificate from file {AttestationCertificateFile}");
                using (FileStream fileStream = AttestationCertificateFile!.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string attestation = reader.ReadToEnd();
                        byte[] decodedBytes = Array.Empty<byte>();
                        if (PemEncoding.TryFind(attestation, out PemFields pemFields))
                        {
                            string base64Payload = attestation.Substring(pemFields.Base64Data.Start.Value, (pemFields.Base64Data.End.Value - pemFields.Base64Data.Start.Value));
                            _AttestationCertificate = new X509Certificate2(Convert.FromBase64String(base64Payload));
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid certificate data in attestation file.");
                        }
                    }
                }

                WriteDebug($"Reading intermediate certificate from file {IntermediateCertificateFile}");
                using (FileStream fileStream = IntermediateCertificateFile!.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string intermediate = reader.ReadToEnd();
                        byte[] decodedBytes = Array.Empty<byte>();
                        if (PemEncoding.TryFind(intermediate, out PemFields pemFields))
                        {
                            string base64Payload = intermediate.Substring(pemFields.Base64Data.Start.Value, (pemFields.Base64Data.End.Value - pemFields.Base64Data.Start.Value));
                            _IntermediateCertificate = new X509Certificate2(Convert.FromBase64String(base64Payload));
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid certificate data in intermediate file.");
                        }
                    }
                }
            }
            else if (ParameterSetName == "CertificateIncludingAttestation-Object")
            {
                _CertificateIncludingAttestation = CertificateIncludingAttestation;
            }
            else if (ParameterSetName == "CertificateIncludingAttestation-File")
            {
                using (FileStream fileStream = CertificateIncludingAttestationFile!.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string attestation = reader.ReadToEnd();
                        match = regex.Match(attestation);
                        if (match.Success)
                        {
                            try
                            {
                                byte[] certificateAttBytes = Convert.FromBase64String(match.Groups["certificateContent"].Value);
                                _CertificateIncludingAttestation = new X509Certificate2(certificateAttBytes);
                            }
                            catch
                            {
                                throw new ArgumentException($"Invalid certificate data.");
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid certificate data.");
                        }
                    }
                }
            }
            #endregion

            #region Process Attestation Data
            // Extract attestation data from built-in certificates
            if ((ParameterSetName == "requestWithBuiltinAttestation-Object" || ParameterSetName == "requestWithBuiltinAttestation-File") && _CertificateRequest is not null)
            {
                // Check for standard attestation OID (1.3.6.1.4.1.41482.3.1)
                if (_CertificateRequest!.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.1"))
                {
                    extensioncsr = _CertificateRequest!.CertificateExtensions
                                       .Cast<X509Extension>()
                                       .FirstOrDefault(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.1", new X509Extension(new AsnEncodedData("1.3.6.1.4.1.41482.3.1", new byte[] { 0x00 }), false));
                    _out_AttestationDataLocation = "1.3.6.1.4.1.41482.3.1";
                    try
                    {
                        _AttestationCertificate = new X509Certificate2(extensioncsr.RawData);
                    }
                    catch
                    {
                        throw new Exception("Failed to parse the embedded attestation certificate");
                    }
                }
                else if (_CertificateRequest!.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.11"))
                {
                    extensioncsr = _CertificateRequest!.CertificateExtensions
                                       .Cast<X509Extension>()
                                       .FirstOrDefault(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.11", new X509Extension(new AsnEncodedData("1.3.6.1.4.1.41482.3.11", new byte[] { 0x00 }), false));
                    _out_AttestationDataLocation = "1.3.6.1.4.1.41482.3.11";
                    try
                    {
                        _AttestationCertificate = new X509Certificate2(extensioncsr.RawData);
                    }
                    catch
                    {
                        throw new Exception("Failed to parse the embedded attestation certificate");
                    }
                }
                else
                {
                    throw new ArgumentException("The CertificateRequest does not contain embedded attestation data");
                }

                if (_CertificateRequest.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.2"))
                {
                    // Read from CSR 1.3.6.1.4.1.41482.3.2
                    extensioncsr = _CertificateRequest.CertificateExtensions
                    .Cast<X509Extension>()
                    .FirstOrDefault(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.2", new X509Extension(new AsnEncodedData("1.3.6.1.4.1.41482.3.2", new byte[] { 0x00 }), false));
                    try
                    {
                        _IntermediateCertificate = new X509Certificate2(extensioncsr.RawData);
                    }
                    catch
                    {
                        throw new ArgumentException("Failed to parse the embedded intermediate attestation certificate");
                    }
                }
                else
                {
                    throw new ArgumentException("The CertificateRequest does not contain embedded intermediate attestation certificate");
                }
            }
            #endregion

            #region Validate Certificate Chain
            // Set up chain validation parameters
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.ExtraStore.Add(_IntermediateCertificate);
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            // Load Yubico root and intermediate certificates from embedded resources
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            // loop through all resources in the assembly
            // If they are in the powershellYK.support.AttestationCerts.CA add to CustomTrustStore
            // If they are in the powershellYK.support.AttestationCerts.Intermediate add to ExtraStore
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith("powershellYK.support.AttestationCerts.CA"))
                {
                    using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream is not null)
                        {
                            byte[] resourceBytes = new byte[resourceStream.Length];
                            resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                            string resourceText = Encoding.UTF8.GetString(resourceBytes);
                            chain.ChainPolicy.CustomTrustStore.ImportFromPem(resourceText.AsSpan());
                        }
                    }
                }
                else if (resourceName.StartsWith("powershellYK.support.AttestationCerts.Intermediate"))
                {
                    using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream is not null)
                        {
                            byte[] resourceBytes = new byte[resourceStream.Length];
                            resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                            string resourceText = Encoding.UTF8.GetString(resourceBytes);
                            chain.ChainPolicy.ExtraStore.ImportFromPem(resourceText.AsSpan());
                        }
                    }
                }
            }
            #endregion

            #region Extract Attestation Information
            if (chain.Build(_AttestationCertificate) == false)
            {
                // Chain validation failed
                Attestation returnObject = new Attestation(false);
                WriteObject(returnObject);
            }
            else
            {
                // Extract PIV slot from certificate subject
                WriteDebug($"Slot {_AttestationCertificate.Subject}");
                string slotPattern = @"CN=YubiKey PIV Attestation (?<slot>[0-9A-Fa-f]{2})";
                Regex slotRegex = new Regex(slotPattern);
                Match slotMatch = slotRegex.Match(_AttestationCertificate.Subject);
                if (slotMatch.Success)
                {
                    _out_Slot = byte.Parse(slotMatch.Groups["slot"].Value, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    _out_Slot = 0x00;
                }

                // Process certificate extensions to extract YubiKey metadata
                foreach (X509Extension extension in _AttestationCertificate.Extensions)
                {
                    switch (extension.Oid!.Value)
                    {
                        case "1.3.6.1.4.1.41482.3.3": // Firmware version
                            WriteDebug("Extracting Firmware version...");
                            _out_FirmwareVersion = new FirmwareVersion(extension.RawData[0], extension.RawData[1], extension.RawData[2]);
                            break;
                        case "1.3.6.1.4.1.41482.3.7": // Serial number
                            WriteDebug("Extracting Serial number...");
                            byte[] tempSerialBytes = extension.RawData;
                            Array.Reverse(tempSerialBytes);
                            _out_SerialNumber = BitConverter.ToUInt32(tempSerialBytes, 0);
                            break;
                        case "1.3.6.1.4.1.41482.3.8": // Pin / Touch Policies
                            WriteDebug("Extracting Pin / Touch Policies...");
                            _out_PinPolicy = (PivPinPolicy)extension.RawData[0];
                            _out_TouchPolicy = (PivTouchPolicy)extension.RawData[1];
                            break;
                        case "1.3.6.1.4.1.41482.3.9": // Form factor
                            WriteDebug("Extracting Form factor...");
                            _out_FormFactor = (FormFactor)(extension.RawData[0] & (byte)0x7F);
                            break;
                    }
                }

                // Determine YubiKey series type (FIPS/CSPN)
                _out_isFIPSSeries = false;
                _out_isCSPNSeries = false;

                foreach (X509Extension extension in _IntermediateCertificate.Extensions)
                {
                    switch (extension.Oid!.Value)
                    {
                        case "1.3.6.1.4.1.41482.3.10":
                            WriteDebug("YubiKey is FIPS series");
                            _out_isFIPSSeries = true;
                            break;
                        case "1.3.6.1.4.1.41482.3.11":
                            WriteDebug("YubiKey is CSPN series");
                            _out_isCSPNSeries = true;
                            break;
                    }
                }

                // Verify public key matches between attestation and CSR/certificate
                if (ParameterSetName.StartsWith("request") && _CertificateRequest is not null)
                {
                    _out_AttestationMatchesCSR = _AttestationCertificate.PublicKey.EncodedKeyValue.RawData.SequenceEqual(_CertificateRequest.PublicKey.EncodedKeyValue.RawData);
                }
                else if (ParameterSetName.StartsWith("Certificate") && _CertificateIncludingAttestation is not null)
                {
                    _out_AttestationMatchesCSR = _AttestationCertificate.PublicKey.EncodedKeyValue.RawData.SequenceEqual(_CertificateIncludingAttestation.PublicKey.EncodedKeyValue.RawData);
                }

                // Determine the public key algorithm and parameters
                if (_AttestationCertificate.PublicKey.Oid.FriendlyName == "RSA")
                {
                    switch (_AttestationCertificate.PublicKey.GetRSAPublicKey()!.KeySize)
                    {
                        case 1024:
                            _out_Algorithm = PivAlgorithm.Rsa1024;
                            break;
                        case 2048:
                            _out_Algorithm = PivAlgorithm.Rsa2048;
                            break;
                        case 3072:
                            _out_Algorithm = PivAlgorithm.Rsa3072;
                            break;
                        case 4096:
                            _out_Algorithm = PivAlgorithm.Rsa4096;
                            break;
                        default:
                            _out_Algorithm = PivAlgorithm.None;
                            break;
                    }
                }
                else if (_AttestationCertificate.PublicKey.Oid.FriendlyName == "ECC")
                {
                    switch (_AttestationCertificate.PublicKey.GetECDsaPublicKey()!.KeySize)
                    {
                        case 256:
                            _out_Algorithm = PivAlgorithm.EccP256;
                            break;
                        case 384:
                            _out_Algorithm = PivAlgorithm.EccP384;
                            break;
                        // For the future :D
                        //                        case 521:
                        //                            _out_Algorithm = PivAlgorithm.EccP521;
                        //                            break;
                        default:
                            _out_Algorithm = PivAlgorithm.None;
                            break;
                    }
                }
                else
                {
                    _out_Algorithm = PivAlgorithm.None;
                }

                // Create and return the attestation object with all extracted data
                Attestation returnObject = new Attestation(
                    true,                           // Validation successful
                    _out_SerialNumber,             // YubiKey serial number
                    _out_FirmwareVersion,          // Firmware version
                    _out_PinPolicy,                // PIN policy
                    _out_TouchPolicy,              // Touch policy
                    _out_FormFactor,               // Form factor
                    _out_Slot,                     // PIV slot
                    _out_Algorithm,                // Key algorithm
                    _out_isFIPSSeries,             // FIPS series flag
                    _out_isCSPNSeries,             // CSPN series flag
                    AttestationMatchesCSR: _out_AttestationMatchesCSR,  // Key matching result
                    attestationDataLocation: _out_AttestationDataLocation // OID location
                );

                WriteObject(returnObject);
            }
            #endregion
        }
    }
}
