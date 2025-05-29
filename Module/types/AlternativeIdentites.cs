/// <summary>
/// Represents alternative identity formats for YubiKey certificates.
/// Provides various formats for certificate identification and authentication.
/// 
/// .EXAMPLE
/// # Create alternative identities from certificate
/// $cert = Get-YubiKeyCertificate
/// $identities = [powershellYK.AlternativeIdentites]::new(
///     $sshKey,
///     $issuerSubject,
///     $subjectOnly,
///     $rfc822,
///     $issuerSerial,
///     $ski,
///     $sha1Key
/// )
/// 
/// .EXAMPLE
/// # Access SSH authorized key format
/// Write-Host $identities.sshAuthorizedkey
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;

namespace powershellYK
{
    // Represents alternative identity formats for YubiKey certificates
    public class AlternativeIdentites
    {
        // SSH authorized key format
        public string? sshAuthorizedkey { get; }

        // X.509 issuer and subject format
        public string? X509IssuerSubject { get; }

        // X.509 subject only format
        public string? X509SubjectOnly { get; }

        // X.509 RFC822 email format
        public string? X509RFC822 { get; }

        // X.509 issuer and serial number format
        public string? X509IssuerSerialNumber { get; }

        // X.509 Subject Key Identifier format
        public string? X509SKI { get; }

        // X.509 SHA1 public key format
        public string? X509SHA1PublicKey { get; }

        // Creates a new instance of alternative identities
        public AlternativeIdentites(
            string? sshAuthorizedKey,
            string? x509IssuerSubject,
            string? x509SubjectOnly,
            string? x509RFC822,
            string? x509IssuerSerialNumber,
            string? x509SKI,
            string? x509SHA1PublicKey)
        {
            sshAuthorizedkey = sshAuthorizedKey;
            X509IssuerSubject = x509IssuerSubject;
            X509SubjectOnly = x509SubjectOnly;
            X509RFC822 = x509RFC822;
            X509IssuerSerialNumber = x509IssuerSerialNumber;
            X509SKI = x509SKI;
            X509SHA1PublicKey = x509SHA1PublicKey;
        }
    }
}
