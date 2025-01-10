using powershellYK.support;

namespace powershellYK.FIDO2
{
    public readonly struct CredentialID
    {
        private readonly byte[] _credentialID;

        public CredentialID(Yubico.YubiKey.Fido2.CredentialId value)
        {
            _credentialID = value.Id.ToArray();
        }
        public CredentialID(powershellYK.FIDO2.Credential value)
        {
            _credentialID = value.CredentialID.ToByte();
        }
        public CredentialID(powershellYK.FIDO2.CredentialID value)
        {
            _credentialID = value.ToByte();
        }
        public CredentialID(byte[] value)
        {
            _credentialID = value;
        }

        //Property to get and set the value

        #region Destinations

        public override string ToString()
        {
            return HexConverter.ByteArrayToString(_credentialID).ToLower();
        }
        public byte[] ToByte()
        {
            return _credentialID;
        }


        #endregion // Destinations

        #region Operators

        public static implicit operator CredentialID(powershellYK.FIDO2.Credential credential)
        {
            return new CredentialID(credential.CredentialID);
        }

        public static implicit operator CredentialID(Yubico.YubiKey.Fido2.CredentialId credentialID)
        {
            return new CredentialID(credentialID);
        }

        public static implicit operator ReadOnlyMemory<byte>(powershellYK.FIDO2.CredentialID credentialID)
        {
            return credentialID.ToByte().AsMemory();
        }

        public static implicit operator byte[](powershellYK.FIDO2.CredentialID credentialID)
        {
            return credentialID._credentialID;
        }

        public static implicit operator CredentialID(string value)
        {
            byte[] credentialID = HexConverter.StringToByteArray(value);
            return new CredentialID(credentialID);
        }

        #endregion // Operators
    }

}
