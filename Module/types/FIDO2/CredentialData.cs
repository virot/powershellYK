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
        private readonly MakeCredentialData _makeCredentialData;
        
public CredentialData(MakeCredentialData MakeCredentialData)
        {
            this._makeCredentialData = MakeCredentialData;
        }
        public override string ToString()
        {
            return this.ToString(null);
        }
        public string ToString(string? format = "x")
        {
            return "";
        }

        public byte[] w3cEncoded ()
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
            return writer.Encode();
        }
    }
}
