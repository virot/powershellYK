using powershellYK.support;
using Yubico.YubiKey.Cryptography;


namespace powershellYK.FIDO2
{
    public class Challenge
    {
        private readonly byte[] _challenge;

        public Challenge(string value)
        {
            // If the length is 32, we assume it's a hex string
            if (value.Length == 32 && value.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')))
            {
                this._challenge = HexConverter.StringToByteArray(value);
            }
            // else we assume it's a base64 string
            else
            {
                this._challenge = System.Convert.FromBase64String(value);
            }
        }
        public Challenge(byte[] value)
        {
            this._challenge = value;
        }

        public static Challenge FakeChallange(string relyingPartyID)
        {
            return new Challenge(BuildFakeClientDataHash(relyingPartyID));
        }
        public override string ToString()
        {
            return this.ToString(null);
        }
        public string ToString(string? format = "x")
        {
            return HexConverter.ByteArrayToString(_challenge).ToLower();
        }
        public byte[] ToByte()
        {
            return _challenge;
        }
        public byte[] CalculateSHA256()
        {
            var digester = CryptographyProviders.Sha256Creator();
            _ = digester.TransformFinalBlock(_challenge, 0, _challenge.Length);
            return digester.Hash!;
        }
        public string UrlEncode()
        {
            var base64 = Convert.ToBase64String(_challenge);
            var urlEncoded = base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
            return urlEncoded;
        }

        #region Operators

        public static implicit operator byte[](Challenge source)
        {
            return source.ToByte();
        }

        public static implicit operator string(Challenge source)
        {
            return source.ToString(null);
        }

        #endregion // Operators

        #region support

        private static byte[] BuildFakeClientDataHash(string relyingPartyId)
        {
            byte[] idBytes = System.Text.Encoding.Unicode.GetBytes(relyingPartyId);

            // Generate a random value to represent the challenge.
            var randomObject = CryptographyProviders.RngCreator();
            byte[] randomBytes = new byte[16];
            randomObject.GetBytes(randomBytes);

            var digester = CryptographyProviders.Sha256Creator();
            _ = digester.TransformBlock(randomBytes, 0, randomBytes.Length, null, 0);
            _ = digester.TransformFinalBlock(idBytes, 0, idBytes.Length);

            return digester.Hash!;
        }
        #endregion // support
    }
}
