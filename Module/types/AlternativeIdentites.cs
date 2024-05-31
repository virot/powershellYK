using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;

namespace VirotYubikey
{
    public class AlternativeIdentites
    {
        public string? sshAuthorized_key { get; set; }
        public string? X509IssuerSubject { get; set; }
        public string? X509SubjectOnly { get; set; }
        public string? X509RFC822 { get; set; }
        public string? X509IssuerSerialNumber { get; set; }
        public string? X509SKI { get; set; } 
        public string? X509SHA1PublicKey { get; set; }
    }
}
