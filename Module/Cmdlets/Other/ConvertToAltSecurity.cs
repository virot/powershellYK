/// <summary>
/// Converts X.509 certificates or certificate requests to alternative security identities.
/// Supports both RSA and ECC public keys, generating various identity formats including SSH keys,
/// X.509 subject/issuer pairs, and subject key identifiers.
/// 
/// .EXAMPLE
/// $cert = Get-Item cert:\CurrentUser\My\1234567890
/// ConvertTo-AltSecurity -Certificate $cert
/// Converts an X.509 certificate to alternative security identities
/// 
/// .EXAMPLE
/// $csr = New-CertificateRequest -Subject "CN=Test" -KeyAlgorithm RSA
/// ConvertTo-AltSecurity -CertificateRequest $csr
/// Converts a certificate request to alternative security identities
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;


namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsData.ConvertTo, "AltSecurity", DefaultParameterSetName = "From Certificate")]
    public class ConvertToConvertToAltSecurityCommand : Cmdlet
    {
        // Parameters for certificate input
        [TransformCertificatePath_Certificate()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "Certificate to extract info from", ParameterSetName = "From Certificate")]
        public PSObject? Certificate { get; set; }

        // Parameters for certificate request input
        [TransformCertificateRequest_Path()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate request", ParameterSetName = "From CertificateRequest")]
        public PSObject? CertificateRequest { get; set; }

        // Private fields for processing
        private X509Certificate2? _certificate = null;
        private CertificateRequest? _certificateRequest = null;

        // Initialize processing
        protected override void BeginProcessing()
        {
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Handle certificate conversion
            if (Certificate is not null)
            {
                WriteDebug($"Trying to load certificate from {Certificate!.BaseObject.GetType().FullName}");
                if (Certificate.BaseObject is X509Certificate2)
                {
                    _certificate = Certificate.BaseObject as X509Certificate2;
                }
                if (_certificate is not null)
                {
                    string sshkey = "";
                    WriteDebug("Certificate successfully loaded.");

                    // Generate SSH key based on public key type
                    if (_certificate.PublicKey.Oid.FriendlyName == "RSA")
                    {
                        WriteDebug("Certificate public key is of type RSA.");
                        sshkey = GenerateIdentifier.SSHIdentifier(_certificate.PublicKey.GetRSAPublicKey()!, _certificate.Subject);
                    }
                    else if (_certificate.PublicKey.Oid.FriendlyName == "ECC")
                    {
                        WriteDebug("Certificate public key is of type ECC.");
                        sshkey = GenerateIdentifier.SSHIdentifier(_certificate.PublicKey.GetECDsaPublicKey()!, _certificate.Subject);
                    }
                    else
                    {
                        throw new Exception("Unknown public key format!");
                    }

                    // Extract the Subject Key Identifier
                    X509Extension? stringSKI;
                    stringSKI = _certificate!.Extensions.Cast<X509Extension>().FirstOrDefault(extension => extension.Oid!.Value == "2.5.29.14");
                    if (stringSKI == null)
                    {
                        stringSKI = new X509SubjectKeyIdentifierExtension();
                    }

                    // Generate serial number in little endian format
                    string? serialNumber;
                    try
                    {
                        serialNumber = $"X509:<I>{_certificate.Issuer}<SR>{Converter.ByteArrayToString(_certificate.GetSerialNumber())}";
                    }
                    catch
                    {
                        serialNumber = null;
                    }

                    // Create alternative identities object
                    AlternativeIdentites alternativeIdentites = new AlternativeIdentites
                    (
                        sshkey,
                        $"X509:<I>{_certificate.Issuer}<S>{_certificate.Subject}",
                        $"X509:<S>{_certificate.Subject}",
                        "", //alternativeIdentites.X509RFC822 = $"X509:<I>{}";
                        serialNumber,
                        $"X509:<SKI>{((X509SubjectKeyIdentifierExtension)stringSKI!).SubjectKeyIdentifier}",
                        $"X509:<SHA1-PUKEY>{_certificate.Thumbprint}"
                    );

                    WriteObject(alternativeIdentites);
                }
                else
                {
                    throw new Exception("Certificate not loaded");
                }
            }
            // Handle certificate request conversion
            else if (CertificateRequest is not null)
            {
                WriteDebug("Trying to build altSecurityIdentities from CSR");
                if (CertificateRequest.BaseObject is CertificateRequest)
                {
                    _certificateRequest = CertificateRequest.BaseObject as CertificateRequest;
                }
                if (_certificateRequest is not null)
                {
                    string sshkey = "";

                    // Generate SSH key based on public key type
                    if (_certificateRequest.PublicKey.Oid.FriendlyName == "RSA")
                    {
                        WriteDebug("Certificate public key is of type RSA.");
                        sshkey = GenerateIdentifier.SSHIdentifier(_certificateRequest.PublicKey.GetRSAPublicKey()!, "");
                    }
                    else if (_certificateRequest.PublicKey.Oid.FriendlyName == "ECC")
                    {
                        WriteDebug("Certificate public key is of type ECC.");
                        sshkey = GenerateIdentifier.SSHIdentifier(_certificateRequest.PublicKey.GetECDsaPublicKey()!, "");
                    }
                    else
                    {
                        throw new Exception("Unknown public key format!");
                    }

                    // Generate subject key identifier
                    X509Extension stringSKI = new X509SubjectKeyIdentifierExtension(_certificateRequest.PublicKey, false);

                    // Create alternative identities object
                    AlternativeIdentites alternativeIdentites = new AlternativeIdentites
                    (
                        sshkey,
                        null,
                        null,
                        null,
                        null,
                        $"X509:<SKI>{((X509SubjectKeyIdentifierExtension)stringSKI!).SubjectKeyIdentifier}",
                        null
                    );

                    WriteObject(alternativeIdentites);
                }
            }
        }

    }
}