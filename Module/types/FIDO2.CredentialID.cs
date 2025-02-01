using powershellYK.support;
using System.Formats.Cbor;
using System.Security.Cryptography;
using Yubico.YubiKey.Fido2;

namespace powershellYK.FIDO2
{
    public readonly struct CredentialID
    {
        private readonly Yubico.YubiKey.Fido2.CredentialId _YCredentialId;

        public CredentialID(Yubico.YubiKey.Fido2.CredentialId value)
        {
            _YCredentialId = value;
        }
        public CredentialID(byte[] value)
        {
            var writer = new CborWriter(CborConformanceMode.Ctap2Canonical);
            writer.WriteStartMap(3);
            writer.WriteTextString("type");
            writer.WriteTextString("public-key");
            writer.WriteTextString("id");
            writer.WriteByteString(value);
            writer.WriteTextString("transports");
            writer.WriteStartArray(0);
            writer.WriteEndArray();
            writer.WriteEndMap();

            _YCredentialId = new Yubico.YubiKey.Fido2.CredentialId(writer.Encode(), out var bytesRead);
        }
        public CredentialID(string value)
        {
            var writer = new CborWriter(CborConformanceMode.Ctap2Canonical);
            writer.WriteStartMap(3);
            writer.WriteTextString("type");
            writer.WriteTextString("public-key");
            writer.WriteTextString("id");
            writer.WriteByteString(Converter.StringToByteArray(value));
            writer.WriteTextString("transports");
            writer.WriteStartArray(0);
            writer.WriteEndArray();
            writer.WriteEndMap();

            _YCredentialId = new Yubico.YubiKey.Fido2.CredentialId(writer.Encode(), out var bytesRead);
        }

        public static CredentialID FromStringBase64URL(string value)
        {
            return new CredentialID(System.Convert.FromBase64String(Converter.RemoveBase64URLSafe(Converter.AddMissingPadding(value))));
        }

        //Property to get and set the value

        #region Destinations

        public override string ToString()
        {
            return Converter.ByteArrayToString(_YCredentialId.Id.ToArray()).ToLower();
        }

        public byte[] ToByte()
        {
            return _YCredentialId.Id.ToArray();
        }

        public Yubico.YubiKey.Fido2.CredentialId ToYubicoFIDO2CredentialID()
        {
            return _YCredentialId;
        }


        #endregion // Destinations

        #region Operators

        public static implicit operator CredentialID(Yubico.YubiKey.Fido2.CredentialId credentialID)
        {
            return new CredentialID(credentialID);
        }

        public static implicit operator Yubico.YubiKey.Fido2.CredentialId(CredentialID credentialID)
        {
            return credentialID.ToYubicoFIDO2CredentialID();
        }

        public static implicit operator ReadOnlyMemory<byte>(powershellYK.FIDO2.CredentialID credentialID)
        {
            return credentialID.ToByte().AsMemory();
        }

        public static implicit operator CredentialID(string value)
        {
            byte[] credentialID = Converter.StringToByteArray(value);
            return new CredentialID(credentialID);
        }

        #endregion // Operators
    }

}
