/// <summary>
/// Helpers for FIDO2 PRF (hmac-secret / hmac-secret-mc) used by file encryption.
/// hmac-secret-mc (firmware 5.8+) returns the PRF during MakeCredential so first-time
/// encryption needs only one user interaction. Older firmware falls back to hmac-secret
/// via GetAssertions.
///
/// .EXAMPLE
/// var result = Fido2PrfHelper.TryCreateCredentialWithHmacSecretMc(session, "prf-encryption", "prf-encryption", salt);
/// if (result is not null) { credId = result.Value.CredentialId; prf = result.Value.PrfOutput; }
///
/// .EXAMPLE
/// byte[] prf = Fido2PrfHelper.GetPrfViaAssertion(session, rpId, credId, salt);
/// </summary>

// Imports
using System.Text;
using System.Text.Json;
using Yubico.YubiKey.Cryptography;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;
using powershellYK.FIDO2;

namespace powershellYK.support.FIDO2
{
    // Helpers for FIDO2 PRF derivation via hmac-secret and hmac-secret-mc
    public static class Fido2PrfHelper
    {
        // Creates a discoverable credential and returns the PRF in one MakeCredential when hmac-secret-mc is supported (firmware 5.8+). Returns null when the extension is not supported so callers can fall back to hmac-secret + GetAssertions.
        public static (byte[] CredentialId, byte[] PrfOutput)? TryCreateCredentialWithHmacSecretMc(
            Fido2Session fido2Session,
            string relyingPartyId,
            string username,
            ReadOnlyMemory<byte> salt)
        {
            if (!fido2Session.AuthenticatorInfo.IsExtensionSupported(Extensions.HmacSecretMc))
            {
                return null;
            }

            var relyingParty = new RelyingParty(relyingPartyId) { Name = relyingPartyId };
            byte[] userId = SyntheticCredentialHelper.GenerateUserID();
            var userEntity = new UserEntity(userId.AsMemory())
            {
                Name = username,
                DisplayName = username,
            };

            var make = new MakeCredentialParameters(relyingParty, userEntity);
            make.AddOption("rk", true);
            make.AddHmacSecretMcExtension(fido2Session.AuthenticatorInfo, salt);
            make.AddAlgorithm("public-key", CoseAlgorithmIdentifier.ES256);

            var challenge = Challenge.CreateSyntheticChallenge(relyingPartyId);
            var clientData = new
            {
                type = "webauthn.create",
                origin = $"https://{relyingParty.Id}",
                challenge = challenge.Base64URLEncode(),
            };
            var clientDataJSON = JsonSerializer.Serialize(clientData);
            var clientDataBytes = Encoding.UTF8.GetBytes(clientDataJSON);
            var digester = CryptographyProviders.Sha256Creator();
            _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);
            make.ClientDataHash = digester.Hash!.AsMemory();

            MakeCredentialData credentialData = fido2Session.MakeCredential(make);
            byte[] prfOutput = credentialData.AuthenticatorData.GetHmacSecretExtension(fido2Session.AuthProtocol);
            byte[] credId = credentialData.AuthenticatorData.CredentialId!.Id.ToArray();
            return (credId, prfOutput);
        }

        // Requests an assertion with hmac-secret and returns the decrypted 32-byte PRF for the given credential and salt.
        public static byte[] GetPrfViaAssertion(
            Fido2Session fido2Session,
            string relyingPartyId,
            byte[] credentialId,
            ReadOnlyMemory<byte> salt)
        {
            var relyingParty = new RelyingParty(relyingPartyId);
            var clientData = new { type = "webauthn.get", origin = $"https://{relyingPartyId}", challenge = Convert.ToBase64String(salt.ToArray()) };
            var clientDataJSON = JsonSerializer.Serialize(clientData);
            var clientDataBytes = Encoding.UTF8.GetBytes(clientDataJSON);
            var digester = CryptographyProviders.Sha256Creator();
            _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);

            var getParams = new GetAssertionParameters(relyingParty, digester.Hash!);
            getParams.RequestHmacSecretExtension(salt);
            getParams.AllowCredential(new CredentialID(credentialId).ToYubicoFIDO2CredentialID());

            IReadOnlyList<GetAssertionData> assertions = fido2Session.GetAssertions(getParams);
            return assertions[0].AuthenticatorData.GetHmacSecretExtension(fido2Session.AuthProtocol);
        }
    }
}
