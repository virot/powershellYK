/// <summary>
/// Helpers for creating FIDO2 synthetic credentials without an IdP.
/// <c>GenerateUserID</c> returns 32 cryptographically random bytes suitable for
/// WebAuthn <c>user.id</c> (spec allows 1-64 bytes; value must not contain PII).
/// </summary>

// Imports
using Yubico.YubiKey.Cryptography;

namespace powershellYK.support.FIDO2
{
    // Helpers for creating FIDO2 synthetic credentials without an IdP
    public static class SyntheticCredentialHelper
    {
        // Generates a 32-byte cryptographically random user ID
        public static byte[] GenerateUserID()
        {
            byte[] userId = new byte[32];
            var rng = CryptographyProviders.RngCreator();
            rng.GetBytes(userId);
            return userId;
        }
    }
}
