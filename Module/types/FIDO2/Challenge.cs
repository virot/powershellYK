/// <summary>
/// Represents a FIDO2 challenge for authentication operations.
/// Handles challenge generation, encoding, and conversion between formats.
/// 
/// .EXAMPLE
/// # Create a challenge from a base64 string
/// $challenge = [powershellYK.FIDO2.Challenge]::new("SGVsbG8gV29ybGQ=")
/// Write-Host $challenge.ToString()
/// 
/// .EXAMPLE
/// # Create a fake challenge for testing
/// $challenge = [powershellYK.FIDO2.Challenge]::FakeChallange("example.com")
/// Write-Host $challenge.Base64URLEncode()
/// </summary>

// Imports
using powershellYK.support;
using Yubico.YubiKey.Cryptography;

namespace powershellYK.FIDO2
{
    // Represents a FIDO2 challenge for authentication operations
    public class Challenge
    {
        // Internal storage for the challenge bytes
        private readonly byte[] _challenge;

        // Creates a challenge from a base64 string
        public Challenge(string value)
        {
            this._challenge = System.Convert.FromBase64String(AddMissingPadding(value));
        }

        // Creates a challenge from a byte array
        public Challenge(byte[] value)
        {
            this._challenge = value;
        }

        // Generates a fake challenge for testing purposes
        public static Challenge FakeChallange(string relyingPartyID)
        {
            return new Challenge(BuildFakeClientDataHash(relyingPartyID));
        }

        // Converts the challenge to a string representation
        public override string ToString()
        {
            return this.ToString(null);
        }

        // Converts the challenge to a string with specified format
        public string ToString(string? format = "x")
        {
            return Converter.ByteArrayToString(_challenge).ToLower();
        }

        // Returns the challenge as a byte array
        public byte[] ToByte()
        {
            return _challenge;
        }

        // Encodes the challenge in base64url format
        public string Base64URLEncode()
        {
            var base64 = Convert.ToBase64String(_challenge);
            var urlEncoded = base64.Replace('+', '-').Replace('/', '_').Replace("=", "");
            return urlEncoded;
        }

        // Creates a challenge from a base64url encoded string
        public static Challenge FromBase64URLEncoded(string input)
        {
            return new Challenge(System.Convert.FromBase64String(RemoveBase64URLSafe(AddMissingPadding(input))));
        }

        #region Operators

        // Implicit conversion to byte array
        public static implicit operator byte[](Challenge source)
        {
            return source.ToByte();
        }

        // Implicit conversion to string
        public static implicit operator string(Challenge source)
        {
            return source.ToString(null);
        }

        #endregion // Operators

        #region Support Methods

        // Converts base64url safe characters back to standard base64
        private static string RemoveBase64URLSafe(string urlsafe)
        {
            return urlsafe.Replace('-', '+').Replace('_', '/');
        }

        // Adds missing padding to base64 string
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

        // Builds a fake client data hash for testing
        private static byte[] BuildFakeClientDataHash(string relyingPartyId)
        {
            // Convert relying party ID to bytes
            byte[] idBytes = System.Text.Encoding.Unicode.GetBytes(relyingPartyId);

            // Generate random challenge bytes
            var randomObject = CryptographyProviders.RngCreator();
            byte[] randomBytes = new byte[16];
            randomObject.GetBytes(randomBytes);

            // Create hash of random bytes and relying party ID
            var digester = CryptographyProviders.Sha256Creator();
            _ = digester.TransformBlock(randomBytes, 0, randomBytes.Length, null, 0);
            _ = digester.TransformFinalBlock(idBytes, 0, idBytes.Length);

            return digester.Hash!;
        }

        #endregion // Support Methods
    }
}
