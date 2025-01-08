using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;

namespace powershellYK.FIDO2
{
    public class Credentials
    {
        public string? RPId { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public CredentialId? CredentialID { get; set; }
        [Hidden]
        public CoseKey? coseKey { get; set; }
    }
}
