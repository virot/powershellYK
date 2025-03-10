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

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsLifecycle.Confirm, "YubiKeyAttestation")]
    public class ConfirmYubikeyAttestationCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithExternalAttestation")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CSR to check", ParameterSetName = "requestWithBuiltinAttestation")]
        public PSObject? CertificateRequest { get; set; }

        [TransformCertificatePath_Certificate()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "requestWithExternalAttestation")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "AttestationCertificate", ParameterSetName = "JustAttestCertificate")]
        public PSObject? AttestationCertificate { get; set; }

        [TransformCertificatePath_Certificate()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "requestWithExternalAttestation")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "IntermediateCertificate", ParameterSetName = "JustAttestCertificate")]
        public PSObject? IntermediateCertificate { get; set; }

        [TransformCertificatePath_Certificate()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "CertificateIncludingAttestation", ParameterSetName = "CertificateIncludingAttestation")]
        public PSObject? CertificateIncludingAttestation { get; set; }


        private CertificateRequest? _CertificateRequest;
        private X509Certificate2? _AttestationCertificate;
        private X509Certificate2? _IntermediateCertificate;
        private X509Certificate2? _CertificateIncludingAttestation;
        private static X509Certificate2 _YubikeyValidationCA = new X509Certificate2(new byte[] { 0x30, 0x82, 0x3, 0x17, 0x30, 0x82, 0x1, 0xFF, 0xA0, 0x3, 0x2, 0x1, 0x2, 0x2, 0x3, 0x4, 0x6, 0x47, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x30, 0x2B, 0x31, 0x29, 0x30, 0x27, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x20, 0x59, 0x75, 0x62, 0x69, 0x63, 0x6F, 0x20, 0x50, 0x49, 0x56, 0x20, 0x52, 0x6F, 0x6F, 0x74, 0x20, 0x43, 0x41, 0x20, 0x53, 0x65, 0x72, 0x69, 0x61, 0x6C, 0x20, 0x32, 0x36, 0x33, 0x37, 0x35, 0x31, 0x30, 0x20, 0x17, 0xD, 0x31, 0x36, 0x30, 0x33, 0x31, 0x34, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x18, 0xF, 0x32, 0x30, 0x35, 0x32, 0x30, 0x34, 0x31, 0x37, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x30, 0x2B, 0x31, 0x29, 0x30, 0x27, 0x6, 0x3, 0x55, 0x4, 0x3, 0xC, 0x20, 0x59, 0x75, 0x62, 0x69, 0x63, 0x6F, 0x20, 0x50, 0x49, 0x56, 0x20, 0x52, 0x6F, 0x6F, 0x74, 0x20, 0x43, 0x41, 0x20, 0x53, 0x65, 0x72, 0x69, 0x61, 0x6C, 0x20, 0x32, 0x36, 0x33, 0x37, 0x35, 0x31, 0x30, 0x82, 0x1, 0x22, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0x1, 0x5, 0x0, 0x3, 0x82, 0x1, 0xF, 0x0, 0x30, 0x82, 0x1, 0xA, 0x2, 0x82, 0x1, 0x1, 0x0, 0xC3, 0x76, 0x70, 0xC4, 0xCD, 0x47, 0xA6, 0x2, 0x75, 0xC4, 0xC5, 0x47, 0x1B, 0x8F, 0xCB, 0x7D, 0x4F, 0x69, 0xB4, 0x67, 0xE6, 0x6E, 0xA9, 0x27, 0xE9, 0xD2, 0x13, 0x41, 0xD1, 0x5A, 0x9A, 0x1A, 0x33, 0xC7, 0xDC, 0xF3, 0x1, 0xC2, 0xF9, 0x39, 0x9B, 0xF7, 0xC8, 0xE6, 0x36, 0xF8, 0x56, 0x34, 0x4D, 0x84, 0x8A, 0x55, 0x3C, 0xE6, 0xE6, 0xA, 0x7C, 0x41, 0x4F, 0xF5, 0xDE, 0x90, 0xD8, 0x69, 0xB2, 0xB6, 0xA0, 0x67, 0xC5, 0x9B, 0x0, 0x6B, 0x72, 0xAA, 0x66, 0x20, 0x82, 0xC7, 0x62, 0xF0, 0x43, 0x88, 0x98, 0x10, 0xE6, 0xF5, 0x96, 0x58, 0x28, 0xB5, 0x5A, 0xFF, 0xC2, 0x11, 0x29, 0x75, 0x53, 0xAA, 0x8E, 0x85, 0x34, 0x3F, 0x97, 0xB5, 0x8F, 0x5C, 0xBB, 0x39, 0xFC, 0xE, 0xBE, 0x4C, 0xBF, 0xF8, 0x5, 0xC8, 0x37, 0xFF, 0x57, 0xA7, 0x45, 0x45, 0x95, 0x84, 0x64, 0xDA, 0xD4, 0x3D, 0x19, 0xC7, 0x58, 0x28, 0x39, 0xAA, 0x53, 0xE7, 0x5B, 0xF6, 0x22, 0xB0, 0xA4, 0xC, 0xE2, 0x77, 0x8A, 0x7, 0x5, 0x52, 0xC8, 0x86, 0x60, 0xF7, 0xA6, 0xF9, 0x16, 0x69, 0x10, 0x36, 0x1F, 0x70, 0xC0, 0xF6, 0xDE, 0xC7, 0xFC, 0x73, 0x6A, 0xE6, 0xFD, 0xCE, 0x88, 0xED, 0x63, 0xC8, 0xB6, 0x5E, 0x2A, 0xA6, 0x68, 0x31, 0xB3, 0xCE, 0x6E, 0xBC, 0x6A, 0xE, 0xF, 0xBD, 0x7C, 0xE7, 0x52, 0x87, 0x38, 0x1F, 0xC0, 0x2A, 0xA0, 0x4F, 0x75, 0xD5, 0x99, 0x37, 0xA2, 0xC2, 0xF0, 0x52, 0x4D, 0xCB, 0x72, 0x8B, 0xD9, 0x87, 0x41, 0xF6, 0x1D, 0xD8, 0x3C, 0x24, 0x6A, 0xAC, 0x51, 0x9C, 0xB6, 0xCD, 0x57, 0x22, 0xBD, 0xCE, 0x5F, 0x83, 0xCE, 0x34, 0x86, 0xA7, 0xD2, 0x21, 0x54, 0xF8, 0x95, 0xB4, 0x67, 0xAD, 0x5F, 0x4D, 0x9D, 0xC6, 0x14, 0x27, 0x19, 0x2E, 0xCA, 0xE8, 0x13, 0xB4, 0x41, 0xEF, 0x2, 0x3, 0x1, 0x0, 0x1, 0xA3, 0x42, 0x30, 0x40, 0x30, 0x1D, 0x6, 0x3, 0x55, 0x1D, 0xE, 0x4, 0x16, 0x4, 0x14, 0xCA, 0x5F, 0xCA, 0xF2, 0xC4, 0xA2, 0x31, 0x9C, 0xE9, 0x22, 0x5F, 0xF1, 0xEC, 0xF4, 0xD5, 0xDF, 0x2, 0xBF, 0x83, 0xBF, 0x30, 0xF, 0x6, 0x3, 0x55, 0x1D, 0x13, 0x4, 0x8, 0x30, 0x6, 0x1, 0x1, 0xFF, 0x2, 0x1, 0x1, 0x30, 0xE, 0x6, 0x3, 0x55, 0x1D, 0xF, 0x1, 0x1, 0xFF, 0x4, 0x4, 0x3, 0x2, 0x1, 0x6, 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0xB, 0x5, 0x0, 0x3, 0x82, 0x1, 0x1, 0x0, 0x5C, 0xEC, 0x88, 0x7C, 0x5, 0xCD, 0x5F, 0x90, 0x2F, 0x85, 0xC8, 0xDD, 0x5F, 0x86, 0x35, 0xA2, 0xA0, 0x10, 0x8C, 0xAF, 0x7B, 0xE3, 0x9D, 0xE8, 0x7B, 0x30, 0xB6, 0xC0, 0xEA, 0x44, 0xA8, 0xC9, 0x61, 0x7B, 0xD0, 0xDD, 0xEC, 0x5E, 0x16, 0xD7, 0xBD, 0x3E, 0x1E, 0x46, 0x1D, 0x21, 0xBF, 0x1A, 0xAF, 0x31, 0x93, 0x63, 0x3D, 0x4F, 0xD5, 0x95, 0x19, 0xFA, 0x80, 0xB5, 0x6D, 0xA0, 0x48, 0xA4, 0xC, 0xBA, 0xD8, 0x15, 0x73, 0x7A, 0x1E, 0x1E, 0x96, 0x9B, 0x2C, 0xB5, 0x19, 0x39, 0xEC, 0xA6, 0x73, 0xAF, 0x32, 0xFC, 0xF6, 0x94, 0xB2, 0xAE, 0xCA, 0x6F, 0x4A, 0x61, 0xD6, 0xB, 0xE, 0x9, 0xE3, 0xDC, 0x17, 0x80, 0xBF, 0x32, 0x21, 0x57, 0x3C, 0xD8, 0x49, 0xE5, 0x3B, 0xEF, 0xF0, 0xAE, 0xA6, 0x87, 0xE3, 0xD3, 0xDD, 0xCE, 0xB8, 0xB, 0x30, 0x5B, 0x48, 0xD8, 0xBD, 0x7B, 0x6, 0x4F, 0x28, 0xB1, 0xE8, 0x1D, 0xDD, 0x6D, 0x6E, 0x72, 0x5A, 0xFC, 0x92, 0xF7, 0x33, 0x57, 0x6A, 0xA1, 0x9A, 0x52, 0x63, 0xF7, 0x53, 0xDF, 0xDB, 0xE8, 0x39, 0x47, 0x74, 0x3A, 0x20, 0x30, 0xBB, 0xB7, 0x54, 0xBA, 0x41, 0x7, 0xD6, 0xE6, 0xE5, 0xB8, 0xDA, 0x29, 0x65, 0x89, 0x62, 0x5, 0xA5, 0xB4, 0x25, 0x60, 0x51, 0xB1, 0x6A, 0x16, 0xAC, 0xA2, 0xE3, 0xE2, 0x44, 0xD3, 0x5E, 0x1C, 0x4A, 0x4, 0x79, 0xEC, 0x97, 0x2E, 0xDD, 0xD6, 0x62, 0x7A, 0x10, 0x7A, 0x52, 0xD0, 0xF, 0x81, 0xA7, 0x7D, 0x2F, 0x97, 0xD, 0xBE, 0xE6, 0xBF, 0x21, 0x64, 0x66, 0x9B, 0xE0, 0xD, 0xCB, 0x73, 0xB6, 0x2C, 0x7F, 0xBE, 0x3F, 0x29, 0x7C, 0x49, 0x11, 0x33, 0x53, 0xCA, 0x27, 0x6C, 0x1B, 0x23, 0x32, 0xF, 0x50, 0xE, 0x24, 0x9F, 0xE6, 0x82, 0x4B, 0x2A, 0xF7, 0x7F, 0x45, 0xE9, 0xFE, 0xCC, 0x66, 0x3B });

        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        // temp storage for the attestation objectdata
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
        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            Regex regex = new Regex(pemFormatRegex, RegexOptions.Singleline);
            Match? match;
            X509Extension? extensioncsr;

            if (ParameterSetName == "requestWithBuiltinAttestation")
            {
                if (CertificateRequest is not null && CertificateRequest.BaseObject is string)
                {
                    WriteDebug("CertificateRequest is string");
                    match = regex.Match(CertificateRequest.BaseObject.ToString()!);
                    if (match.Success)
                    {
                        WriteDebug("CertificateRequest is PEM string");
                        _CertificateRequest = LoadSigningRequestPem((string)CertificateRequest.BaseObject, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                    }
                    else // Check if it is a file or throw an error.
                    {
                        // there is a bug with GetCurrentDirectory, which breaks relative paths
                        // Should probably really revert to the incorrect path after this.
                        System.IO.Directory.SetCurrentDirectory(this.SessionState.Path.CurrentLocation.ToString());
                        if (System.IO.File.Exists(CertificateRequest.BaseObject.ToString()))
                        {
                            WriteDebug("CertificateRequest is filepath");
                            string CertificateRequestString = System.IO.File.ReadAllText((string)CertificateRequest!.BaseObject);
                            _CertificateRequest = LoadSigningRequestPem(CertificateRequestString, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                        }
                        else
                        {
                            throw new ArgumentException("String must contain PEM certificate request or path to same.", "CertificateRequest");
                        }
                    }
                }
                else if (CertificateRequest is not null && CertificateRequest.BaseObject is byte[])
                {
                    WriteDebug("CertificateRequest is byte[]");
                    _CertificateRequest = LoadSigningRequest((byte[])CertificateRequest.BaseObject, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                }

                // If the certificateRequest contains the attestation certificate, extract it.
                // Due to yubico-piv-tool not storing it in the correct place, we need to check both places.
                // https://docs.yubico.com/hardware/oid/oid-piv-arc.html
                if (_CertificateRequest!.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.11"))
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
                else if (_CertificateRequest!.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.1"))
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
                        throw new ArgumentException("Failed to parse the embedded attestation certificate");
                    }
                }
                else
                {
                    throw new ArgumentException("The CertificateRequest does not contain embedded attestation data");
                }

                if (_CertificateRequest!.CertificateExtensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.2"))
                {
                    // Read from CSR 1.3.6.1.4.1.41482.3.2
                    extensioncsr = _CertificateRequest!.CertificateExtensions
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
            else if (ParameterSetName == "CertificateIncludingAttestation")
            {
                if (CertificateIncludingAttestation is not null && CertificateIncludingAttestation.BaseObject is X509Certificate2)
                {
                    _CertificateIncludingAttestation = (X509Certificate2)CertificateIncludingAttestation.BaseObject;


                    if (_CertificateIncludingAttestation.Extensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.1"))
                    {
                        WriteDebug("Found 1.3.6.1.4.1.41482.3.2 extension, trying to extract attestationdata certificate");
                        extensioncsr = _CertificateIncludingAttestation.Extensions.Cast<X509Extension>().FirstOrDefault(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.1", new X509Extension(new AsnEncodedData("1.3.6.1.4.1.41482.3.1", new byte[] { 0x00 }), false));
                        _out_AttestationDataLocation = "1.3.6.1.4.1.41482.3.1";
                        try
                        {
                            _AttestationCertificate = new X509Certificate2(extensioncsr.RawData);
                        }
                        catch
                        {
                            throw new ArgumentException("Failed to parse the embedded attestationdata certificate");
                        }
                    }
                    else
                    {
                        throw new Exception("Certificate does not contain attestation data");
                    }

                    if (_CertificateIncludingAttestation!.Extensions.Any(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.2"))
                    {
                        WriteDebug("Found 1.3.6.1.4.1.41482.3.2 extension, trying to extract intermediate certificate");
                        // Read from CSR 1.3.6.1.4.1.41482.3.2
                        extensioncsr = _CertificateIncludingAttestation.Extensions.Cast<X509Extension>().FirstOrDefault(e => e.Oid!.Value == "1.3.6.1.4.1.41482.3.2", new X509Extension(new AsnEncodedData("1.3.6.1.4.1.41482.3.2", new byte[] { 0x00 }), false));
                        try
                        {
                            _IntermediateCertificate = new X509Certificate2(extensioncsr.RawData);
                        }
                        catch
                        {
                            throw new ArgumentException("Failed to parse the embedded intermediate attestation certificate");
                        }

                    }
                }
                else
                {
                    throw new ArgumentException("Must be of type string or X509Certificate2", "CertificateWithAttestationData");
                }
            }
            else if (ParameterSetName == "JustAttestCertificate")
            {
                if (IntermediateCertificate!.BaseObject is X509Certificate2)
                {
                    WriteDebug("IntermediateCertificate is X509Certificate2");
                    _IntermediateCertificate = (X509Certificate2)IntermediateCertificate.BaseObject;
                }
                else
                {
                    throw new ArgumentException("Must be of type string or X509Certificate2", "IntermediateCertificate");
                }

                if (AttestationCertificate!.BaseObject is X509Certificate2)
                {
                    WriteDebug("AttestationCertificate is X509Certificate2");
                    _AttestationCertificate = (X509Certificate2)AttestationCertificate.BaseObject;
                }
                else
                {
                    throw new ArgumentException("Must be of type string or X509Certificate2", "AttestationCertificate");
                }
            }
            else
            {
                throw new Exception("Invalid ParameterSetName");
            }

            if (_AttestationCertificate is null || _IntermediateCertificate is null)
            {
                // Is this still needed??
                throw new Exception("Attestation Certificate or Intermediate Certificate is missing!");
            }

            // Check the entire chain up to Yubico's root CA
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.CustomTrustStore.Add(_YubikeyValidationCA);
            chain.ChainPolicy.ExtraStore.Add(_IntermediateCertificate);
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            if (chain.Build(_AttestationCertificate) == false)
            {
                Attestation returnObject = new Attestation(false);
                WriteObject(returnObject);
            }
            else
            {
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

                // Make sure that the default type is Normal.
                _out_isFIPSSeries = false;
                _out_isCSPNSeries = false;

                foreach (X509Extension extension in _IntermediateCertificate.Extensions)
                {
                    switch (extension.Oid!.Value)
                    {
                        case "1.3.6.1.4.1.41482.3.10":
                            WriteDebug("Yubikey is FIPS series");
                            _out_isFIPSSeries = true;
                            break;
                        case "1.3.6.1.4.1.41482.3.11":
                            WriteDebug("Yubikey is CSPN series");
                            _out_isCSPNSeries = true;
                            break;
                    }
                }

                // Check if the public key in the CSR matches the public key in the attestation certificate
                if (ParameterSetName == "requestWithBuiltinAttestation" && _CertificateRequest is not null)
                {
                    _out_AttestationMatchesCSR = _AttestationCertificate.PublicKey.EncodedKeyValue.RawData.SequenceEqual(_CertificateRequest.PublicKey.EncodedKeyValue.RawData);
                }
                else if (ParameterSetName == "CertificateIncludingAttestation" && _CertificateIncludingAttestation is not null)
                {
                    _out_AttestationMatchesCSR = _AttestationCertificate.PublicKey.EncodedKeyValue.RawData.SequenceEqual(_CertificateIncludingAttestation.PublicKey.EncodedKeyValue.RawData);
                }

                // Figure out the PublicKey algorithm
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

                Attestation returnObject = new Attestation(true, _out_SerialNumber, _out_FirmwareVersion, _out_PinPolicy, _out_TouchPolicy, _out_FormFactor, _out_Slot, _out_Algorithm, _out_isFIPSSeries, _out_isCSPNSeries, AttestationMatchesCSR: _out_AttestationMatchesCSR, attestationDataLocation: _out_AttestationDataLocation);

                WriteObject(returnObject);
            }
        }

    }
}
