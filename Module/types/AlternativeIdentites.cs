using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;

namespace powershellYK
{
    public class AlternativeIdentites
    {
        public string? sshAuthorizedkey { get; }
        public string? X509IssuerSubject { get; }
        public string? X509SubjectOnly { get; }
        public string? X509RFC822 { get; }
        public string? X509IssuerSerialNumber { get;}
        public string? X509SKI { get; } 
        public string? X509SHA1PublicKey { get; }


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
