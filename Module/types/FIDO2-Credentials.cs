using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;

namespace VirotYubikey.FIDO2
{
    public class Credentials
    {
        public string? Site { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
//        public CredentialId? CredentialID { get; set; }

    }
}
