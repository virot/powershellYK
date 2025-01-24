using powershellYK.support;
using System.Formats.Cbor;
using System.Runtime.CompilerServices;
using System.Text;
using Yubico.YubiKey.Fido2;

namespace powershellYK.FIDO2
{
    public class CredentialData
    {
        public MakeCredentialData MakeCredentialData { get { return this._makeCredentialData; } }
        public string ClientDataJSON { get { return this._clientDataJSON; } }

        private readonly string _clientDataJSON;
        private readonly MakeCredentialData _makeCredentialData;
        private readonly UserEntity _userEntity;
        private readonly RelyingParty _relyingParty;

        public CredentialData(MakeCredentialData MakeCredentialData, string clientData, UserEntity userEntity, RelyingParty relyingParty)
        {
            this._makeCredentialData = MakeCredentialData;
            this._clientDataJSON = clientData;
            this._userEntity = userEntity;
            this._relyingParty = relyingParty;
        }

        public override string ToString()
        {
            var username = _userEntity.DisplayName ?? _userEntity.Name ?? "Unknown";
            var site = _relyingParty.Name ?? _relyingParty.Id ?? "Unknown";
            return $"Credential for {username} on {site}";
        }
        public string GetBase64clientDataJSON()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this._clientDataJSON));
        }
        public string GetBase64AttestationObject()
        {
            var writer = new CborWriter();
            writer.WriteStartMap(3);
            writer.WriteTextString("fmt");
            writer.WriteTextString(this._makeCredentialData.Format);
            writer.WriteTextString("attStmt");
            writer.WriteEncodedValue(_makeCredentialData.EncodedAttestationStatement.Span);
            writer.WriteTextString("authData");
            writer.WriteByteString(_makeCredentialData.AuthenticatorData.EncodedAuthenticatorData.Span);
            writer.WriteEndMap();
            var cborEncoded = writer.Encode();
            return Convert.ToBase64String(cborEncoded);
        }

        public string GetBase64UrlSafeCredentialID()
        {
            // Return the CredentialID as a base64url string
            return Convert.ToBase64String(this._makeCredentialData.AuthenticatorData.CredentialId!.Id.ToArray()).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
    }
}
