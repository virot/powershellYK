/// <summary>
/// Represents the key material and signature produced by the FIDO2 previewSign extension.
/// This is the primary pipeline output of New-YubiKeyFIDO2Signature: it carries the
/// ARKG seed, key handle, the digest that was signed and the on-device signature, so
/// callers can store/inspect the material and re-sign new data later.
///
/// The signature is produced on the YubiKey via a GetAssertion previewSign request.
/// The key handle is retained so the same key can sign additional payloads without
/// re-provisioning. ARKGSeed is the ARKG-P256 seed used to derive a signing ticket.
/// DerivedPublicKey is the ESP256 P-256 key that verifies Signature. Confirm-YubiKeyFIDO2Signature
/// checks the signature, an optional re-hash of the original data, and decodes the
/// generated-key attestation object.
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

        // ARKG-P256 seed COSE key bytes (hidden; use ARKGSeed)
        [Hidden]
        public byte[]? ARKGSeedCose { get; private set; }

        // CBOR attestation object for the generated signing key (hidden; use AttestationObject)
        [Hidden]
        public byte[]? AttestationObjectBytes { get; private set; }

        // Digest of the input that was signed (hidden; use ToBeSigned)
        [Hidden]
        public byte[] ToBeSignedBytes { get; private set; }

        // Derived ESP256 P-256 public key for this signature (hidden; use DerivedPublicKey)
        [Hidden]
        public byte[]? DerivedPublicKeySec1 { get; private set; }

        // On-device previewSign signature over ToBeSigned (hidden; use Signature)
        [Hidden]
        public byte[]? SignatureBytes { get; private set; }

        // Base64URL rendering of the ARKG-P256 seed (not the ECDSA verify key)
        public string? ARKGSeed => ARKGSeedCose is null ? null : Base64Url(ARKGSeedCose);

        // Base64URL rendering of the key handle
        public string? KeyHandle => KeyHandleBytes is null ? null : Base64Url(KeyHandleBytes);

        // Base64URL rendering of the signing-key attestation object
        public string? AttestationObject => AttestationObjectBytes is null ? null : Base64Url(AttestationObjectBytes);

        // Hex rendering of the digest that was signed
        public string ToBeSigned => Convert.ToHexString(ToBeSignedBytes).ToLowerInvariant();

        // Base64URL rendering of the derived ESP256 public key that verifies Signature
        public string? DerivedPublicKey => DerivedPublicKeySec1 is null ? null : Base64Url(DerivedPublicKeySec1);

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
            byte[]? arkgSeedCose = null,
            byte[]? attestationObject = null,
            byte[]? signature = null,
            byte[]? derivedPublicKey = null)
        {
            this.CredentialID = credentialID;
            this.RelyingPartyID = relyingPartyID;
            this.AlgorithmIdentifier = algorithm;
            this.HashAlgorithm = hashAlgorithm;
            this.ToBeSignedBytes = toBeSigned;
            this.KeyHandleBytes = keyHandle;
            this.ARKGSeedCose = arkgSeedCose;
            this.AttestationObjectBytes = attestationObject;
            this.SignatureBytes = signature;
            this.DerivedPublicKeySec1 = derivedPublicKey;
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
                ARKGSeed = this.ARKGSeed,
                KeyHandle = this.KeyHandle,
                AttestationObject = this.AttestationObject,
                ToBeSigned = this.ToBeSigned,
                DerivedPublicKey = this.DerivedPublicKey,
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
                // Prefer ARKGSeed; accept legacy PublicKey from older host stores
                string? arkgSeed = OptionalString(root, "ARKGSeed") ?? OptionalString(root, "PublicKey");
                string? keyHandle = OptionalString(root, "KeyHandle");
                string? attestationObject = OptionalString(root, "AttestationObject");
                string? toBeSignedHex = OptionalString(root, "ToBeSigned");
                string? derivedPublicKey = OptionalString(root, "DerivedPublicKey");
                string? signature = OptionalString(root, "Signature");

                if (string.IsNullOrWhiteSpace(keyHandle))
                {
                    throw new ArgumentException("The previewSign key JSON does not contain KeyHandle.");
                }
                if (string.IsNullOrWhiteSpace(arkgSeed))
                {
                    throw new ArgumentException("The previewSign key JSON does not contain ARKGSeed.");
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
                    arkgSeedCose: Converter.Base64UrlToByteArray(arkgSeed),
                    attestationObject: string.IsNullOrWhiteSpace(attestationObject) ? null : Converter.Base64UrlToByteArray(attestationObject),
                    signature: string.IsNullOrWhiteSpace(signature) ? null : Converter.Base64UrlToByteArray(signature),
                    derivedPublicKey: string.IsNullOrWhiteSpace(derivedPublicKey) ? null : Converter.Base64UrlToByteArray(derivedPublicKey));
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
            return this.ARKGSeedCose is not null
                ? $"previewSign key for RP '{rp}' ({this.Algorithm}), ARKG seed {this.ARKGSeedCose.Length} bytes"
                : $"previewSign context for RP '{rp}' (no key material; create a credential to generate one)";
        }

        // Encode bytes as base64url (no padding), matching other FIDO2 types
        private static string Base64Url(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
    }
}
