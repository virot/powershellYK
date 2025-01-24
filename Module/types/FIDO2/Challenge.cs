using powershellYK.support;
using Yubico.YubiKey.Cryptography;


namespace powershellYK.FIDO2
{
    public class Challenge
    {
        private readonly byte[] _challenge;

        public Challenge(string value)
        {
            this._challenge = System.Convert.FromBase64String(AddMissingPadding(value));
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
        public string Base64URLEncode()
        {
            var base64 = Convert.ToBase64String(_challenge);
            var urlEncoded = base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
            return urlEncoded;
        }

        public static Challenge FromBase64URLEncoded(string input)
        {
            return new Challenge(System.Convert.FromBase64String(RemoveBase64URLSafe(AddMissingPadding(input))));
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
        private static string RemoveBase64URLSafe(string urlsafe)
        {
            return urlsafe.Replace('-', '+').Replace('_', '/');
        }

        private static string AddMissingPadding(string base64)
        {
            // Calculate the number of padding characters needed
            int paddingCount = 4 - (base64.Length % 4);
            if (paddingCount != 4)
            {
                base64 += new string('=', paddingCount);
            }
            return base64;
        }
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
