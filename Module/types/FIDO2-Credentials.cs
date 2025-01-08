using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;

namespace powershellYK.FIDO2
{
    public class Credentials
    {
        public string? CredentialID { get; set; }
        public string? RPId { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }

    }
}
