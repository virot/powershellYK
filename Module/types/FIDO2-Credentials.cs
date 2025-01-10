using Newtonsoft.Json.Linq;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;

namespace powershellYK.FIDO2
{
    public class Credential
    {
        public string? DisplayName { get; private set; }
        public string? UserName { get; private set; }
        public string? RPId { get; private set; }
        public powershellYK.FIDO2.CredentialID CredentialID { get; private set; }
        [Hidden]
        public CoseKey? coseKey { get; set; }

        public Credential(string RPId, string? UserName, string? DisplayName, CredentialId CredentialID)
        {
            this.RPId = RPId;
            this.UserName = UserName;
            this.DisplayName = DisplayName;
            this.CredentialID = new powershellYK.FIDO2.CredentialID(CredentialID);
        }

        #region Operators

        #endregion // Operators
    }
}
