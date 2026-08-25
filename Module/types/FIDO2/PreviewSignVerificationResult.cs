/// <summary>
/// Result of offline previewSign signature verification from Confirm-YubiKeyFIDO2Signature.
/// Valid is true when the ECDSA signature checks and, if original data was supplied,
/// the digest of that data matches ToBeSigned. Attestation is decoded for inspection
/// (firmware 5.8 uses fmt=none, which is not a packed Yubico certificate chain).
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

        // Base64URL derived ESP256 public key used for verification
        public string? DerivedPublicKey { get; }

        // Digest algorithm recorded on the PreviewSignKey
        public string HashAlgorithm { get; }

        // Relying party id recorded on the PreviewSignKey
        public string? RelyingPartyID { get; }

        // True when the attestation object CBOR parsed
        public bool AttestationDecoded { get; }

        // Attestation statement format (e.g. none)
        public string? AttestationFormat { get; }

        // Whether SHA-256(RP id) matches authenticator data; null when not comparable
        public bool? RpIdHashMatches { get; }

        // Authenticator AAGUID from attestation authData
        public string? Aaguid { get; }

        // User presence flag from attestation authData
        public bool? UserPresence { get; }

        // User verification flag from attestation authData
        public bool? UserVerification { get; }

        // Signature counter from attestation authData
        public uint? SignatureCounter { get; }

        // CLR type name of the parsed attestation statement
        public string? AttestationStatementType { get; }

        // Short reasons (fmt=none, digest skipped, old store, decode failure)
        public List<string> Notes { get; }

        // Creates a verification result
        public PreviewSignVerificationResult(
            bool signatureValid,
            bool? digestMatches,
            string? derivedPublicKey,
            string hashAlgorithm,
            string? relyingPartyID,
            bool attestationDecoded,
            string? attestationFormat,
            bool? rpIdHashMatches,
            string? aaguid,
            bool? userPresence,
            bool? userVerification,
            uint? signatureCounter,
            string? attestationStatementType,
            List<string>? notes = null)
        {
            this.SignatureValid = signatureValid;
            this.DigestMatches = digestMatches;
            this.Valid = signatureValid && digestMatches != false;
            this.DerivedPublicKey = derivedPublicKey;
            this.HashAlgorithm = hashAlgorithm;
            this.RelyingPartyID = relyingPartyID;
            this.AttestationDecoded = attestationDecoded;
            this.AttestationFormat = attestationFormat;
            this.RpIdHashMatches = rpIdHashMatches;
            this.Aaguid = aaguid;
            this.UserPresence = userPresence;
            this.UserVerification = userVerification;
            this.SignatureCounter = signatureCounter;
            this.AttestationStatementType = attestationStatementType;
            this.Notes = notes ?? new List<string>();
        }

        // String representation
        public override string ToString()
        {
            string digest = this.DigestMatches is null ? "skipped" : this.DigestMatches.Value.ToString().ToLowerInvariant();
            return $"previewSign verification Valid={this.Valid}, SignatureValid={this.SignatureValid}, DigestMatches={digest}";
        }
    }
}
