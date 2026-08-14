/// <summary>
/// Represents the key material and signature produced by the FIDO2 previewSign extension.
/// This is the primary pipeline output of New-YubiKeyFIDO2Signature: it carries the
/// generated public key, key handle, the digest that was signed and the on-device
/// signature, so callers can store/inspect the material and re-sign new data later.
///
/// The signature is produced on the YubiKey via a GetAssertion previewSign request.
/// The key handle is retained so the same key can sign additional payloads without
/// re-provisioning. Offline ARKG-P256 verification of the signature (deriving the
/// ESP256 verify key) is out of scope - that glue ships only in TestUtilities.
///
/// .EXAMPLE
/// $key = New-YubiKeyFIDO2Signature -InputData (Get-Content .\data.bin -AsByteStream -Raw)
/// $key.Signature
///
/// .EXAMPLE
/// $key | New-YubiKeyFIDO2Signature -InputData "deadbeef"
/// Re-signs new data with the previously provisioned key handle.
/// </summary>

// Imports
using System.Management.Automation;
using System.Text.Json;
using Yubico.YubiKey.Fido2.Cose;
using powershellYK.support;
using powershellYK.support.FIDO2;

namespace powershellYK.FIDO2
{
    // Output object describing previewSign key material + the digest to be signed
    public class PreviewSignKey
    {
        // Parent FIDO2 credential id (used to build the assertion allow-list)
        public powershellYK.FIDO2.CredentialID CredentialID { get; private set; }

        // Relying party the credential belongs to
        public string? RelyingPartyID { get; private set; }

        // Display name of the signing algorithm (ARKG-P256 / ESP256-split)
        public string Algorithm => PreviewSign.FormatAlgorithm(AlgorithmIdentifier);

        // Digest algorithm used to produce ToBeSigned
        public string HashAlgorithm { get; private set; }

        // Raw COSE algorithm identifier (hidden; use Algorithm)
        [Hidden]
        public CoseAlgorithmIdentifier AlgorithmIdentifier { get; private set; }

        // Raw key handle returned by the YubiKey (hidden; use KeyHandle)
        [Hidden]
        public byte[]? KeyHandleBytes { get; private set; }

        // COSE/ARKG public key bytes (hidden; use PublicKey)
        [Hidden]
        public byte[]? PublicKeyCose { get; private set; }

        // Optional attestation object bytes (hidden; omitted from default view and JSON)
        [Hidden]
        public byte[]? AttestationObject { get; private set; }

        // Digest of the input that was signed (hidden; use ToBeSigned)
        [Hidden]
        public byte[] ToBeSignedBytes { get; private set; }

        // On-device previewSign signature over ToBeSigned (hidden; use Signature)
        [Hidden]
        public byte[]? SignatureBytes { get; private set; }

        // Base64URL rendering of the generated public key
        public string? PublicKey => PublicKeyCose is null ? null : Base64Url(PublicKeyCose);

        // Base64URL rendering of the key handle
        public string? KeyHandle => KeyHandleBytes is null ? null : Base64Url(KeyHandleBytes);

        // Hex rendering of the digest that was signed
        public string ToBeSigned => Convert.ToHexString(ToBeSignedBytes).ToLowerInvariant();

        // Base64URL rendering of the signature
        public string? Signature => SignatureBytes is null ? null : Base64Url(SignatureBytes);

        // Hex rendering of the signature
        public string? SignatureHex => SignatureBytes is null ? null : Convert.ToHexString(SignatureBytes).ToLowerInvariant();

        // Creates a new previewSign key material object
        public PreviewSignKey(
            powershellYK.FIDO2.CredentialID credentialID,
            string? relyingPartyID,
            CoseAlgorithmIdentifier algorithm,
            string hashAlgorithm,
            byte[] toBeSigned,
            byte[]? keyHandle = null,
            byte[]? publicKeyCose = null,
            byte[]? attestationObject = null,
            byte[]? signature = null)
        {
            this.CredentialID = credentialID;
            this.RelyingPartyID = relyingPartyID;
            this.AlgorithmIdentifier = algorithm;
            this.HashAlgorithm = hashAlgorithm;
            this.ToBeSignedBytes = toBeSigned;
            this.KeyHandleBytes = keyHandle;
            this.PublicKeyCose = publicKeyCose;
            this.AttestationObject = attestationObject;
            this.SignatureBytes = signature;
        }

        // Serializes the key material to indented JSON for -OutFile (public names, Format-List order)
        public string ToJson()
        {
            var payload = new
            {
                CredentialID = this.CredentialID.ToString(),
                RelyingPartyID = this.RelyingPartyID,
                Algorithm = this.Algorithm,
                HashAlgorithm = this.HashAlgorithm,
                PublicKey = this.PublicKey,
                KeyHandle = this.KeyHandle,
                ToBeSigned = this.ToBeSigned,
                Signature = this.Signature,
                SignatureHex = this.SignatureHex,
            };
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        }

        // Reconstruct key material from ToJson() / the default host store
        public static PreviewSignKey FromJson(string json)
        {
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("The previewSign key JSON is not valid.", ex);
            }

            using (document)
            {
                JsonElement root = document.RootElement;
                string credentialHex = RequireString(root, "CredentialID");
                string? relyingPartyID = OptionalString(root, "RelyingPartyID");
                CoseAlgorithmIdentifier algorithm = PreviewSign.ParseAlgorithm(OptionalString(root, "Algorithm"));
                string hashAlgorithm = OptionalString(root, "HashAlgorithm") ?? "SHA256";
                string? publicKey = OptionalString(root, "PublicKey");
                string? keyHandle = OptionalString(root, "KeyHandle");
                string? toBeSignedHex = OptionalString(root, "ToBeSigned");
                string? signature = OptionalString(root, "Signature");

                if (string.IsNullOrWhiteSpace(keyHandle))
                {
                    throw new ArgumentException("The previewSign key JSON does not contain KeyHandle.");
                }
                if (string.IsNullOrWhiteSpace(publicKey))
                {
                    throw new ArgumentException("The previewSign key JSON does not contain PublicKey.");
                }

                byte[] toBeSigned = string.IsNullOrWhiteSpace(toBeSignedHex)
                    ? Array.Empty<byte>()
                    : Converter.StringToByteArray(toBeSignedHex);

                return new PreviewSignKey(
                    credentialID: new powershellYK.FIDO2.CredentialID(credentialHex),
                    relyingPartyID: relyingPartyID,
                    algorithm: algorithm,
                    hashAlgorithm: hashAlgorithm,
                    toBeSigned: toBeSigned,
                    keyHandle: Converter.Base64UrlToByteArray(keyHandle),
                    publicKeyCose: Converter.Base64UrlToByteArray(publicKey),
                    signature: string.IsNullOrWhiteSpace(signature) ? null : Converter.Base64UrlToByteArray(signature));
            }
        }

        // Required JSON string property
        private static string RequireString(JsonElement root, string name)
        {
            string? value = OptionalString(root, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The previewSign key JSON does not contain {name}.");
            }
            return value;
        }

        // Optional JSON string property
        private static string? OptionalString(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out JsonElement property) || property.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            return property.GetString();
        }

        // String representation
        public override string ToString()
        {
            string rp = this.RelyingPartyID ?? "Unknown";
            if (this.SignatureBytes is not null)
            {
                return $"previewSign signature for RP '{rp}' ({this.Algorithm}), {this.SignatureBytes.Length}-byte signature over {this.HashAlgorithm} digest";
            }
            return this.PublicKeyCose is not null
                ? $"previewSign key for RP '{rp}' ({this.Algorithm}), public key {this.PublicKeyCose.Length} bytes"
                : $"previewSign context for RP '{rp}' (no key material; create a credential to generate one)";
        }

        // Encode bytes as base64url (no padding), matching other FIDO2 types
        private static string Base64Url(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
    }
}
