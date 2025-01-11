using Newtonsoft.Json.Linq;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;

namespace powershellYK.FIDO2
{
    public class Credential
    {
        public string? DisplayName { get { return this.CredentialUserInfo.User.DisplayName; } }
        public string? UserName { get { return this.CredentialUserInfo.User.Name; } }
        public string? RPId { get { return this.RelyingParty.Id; } }
        public powershellYK.FIDO2.CredentialID CredentialID { get; private set; }
        [Hidden]
        public RelyingParty RelyingParty { get; private set; }
        [Hidden]
        public CredentialUserInfo CredentialUserInfo { get; private set; }

        public Credential(RelyingParty relyingParty, CredentialUserInfo credentialUserInfo)
        {
            this.CredentialID = new powershellYK.FIDO2.CredentialID(credentialUserInfo.CredentialId);
            this.RelyingParty = relyingParty;
            this.CredentialUserInfo = credentialUserInfo;
        }

        #region Operators

        #endregion // Operators
    }
}
