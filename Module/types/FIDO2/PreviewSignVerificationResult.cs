/// <summary>
/// Result of offline previewSign signature verification from Confirm-YubiKeyFIDO2Signature.
/// Valid is true when the ECDSA signature checks and, if original data was supplied,
/// the digest of that data matches ToBeSigned. The generated-key attestation is decoded
/// to surface the authenticator AAGUID and the signing key's UP/UV policy.
///
/// .EXAMPLE
/// $key | Confirm-YubiKeyFIDO2Signature -InputData "data"
/// if ($result.Valid) { "signature and digest match" }
///
/// .EXAMPLE
/// Confirm-YubiKeyFIDO2Signature -JsonPath ~/.powershellYK/previewsign-37290609.json
/// Verifies the stored signature without re-hashing the original document.
/// </summary>

namespace powershellYK.FIDO2
{
    // Output object for Confirm-YubiKeyFIDO2Signature
    public class PreviewSignVerificationResult
    {
        // True when SignatureValid and DigestMatches is not false
        public bool Valid { get; }

        // ECDSA-P256 verification of Signature over ToBeSigned with DerivedPublicKey
        public bool SignatureValid { get; }

        // Whether a re-hash of -InputData / -Path matches ToBeSigned; null when skipped
        public bool? DigestMatches { get; }

        // Authenticator AAGUID from attestation authData
        public string? Aaguid { get; }

        // Signing key requires user presence (previewSign flags policy); null when the flags output is absent
        public bool? UserPresence { get; }

        // Signing key requires user verification (previewSign flags policy); null when the flags output is absent
        public bool? UserVerification { get; }

        // Creates a verification result
        public PreviewSignVerificationResult(
            bool signatureValid,
            bool? digestMatches,
            string? aaguid,
            bool? userPresence,
            bool? userVerification)
        {
            this.SignatureValid = signatureValid;
            this.DigestMatches = digestMatches;
            this.Valid = signatureValid && digestMatches != false;
            this.Aaguid = aaguid;
            this.UserPresence = userPresence;
            this.UserVerification = userVerification;
        }

        // String representation
        public override string ToString()
        {
            string digest = this.DigestMatches is null ? "skipped" : this.DigestMatches.Value.ToString().ToLowerInvariant();
            return $"previewSign verification Valid={this.Valid}, SignatureValid={this.SignatureValid}, DigestMatches={digest}";
        }
    }
}
