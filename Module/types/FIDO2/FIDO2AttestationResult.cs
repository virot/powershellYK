/// <summary>
/// Represents the result of FIDO2 attestation verification.
/// 
/// .EXAMPLE
/// Confirm-YubiKeyFIDO2Attestation -AttestationObject "attestation.bin"
/// if ((Confirm-YubiKeyFIDO2Attestation).AttestationValidated) { Write-Host "Valid" }
/// </summary>

namespace powershellYK.FIDO2
{
    /// <summary>
    /// Result of FIDO2 attestation verification.
    /// </summary>
    public class FIDO2AttestationResult
    {
        /// <summary>
        /// Whether the attestation was validated (certificate chain verified to a trusted root).
        /// </summary>
        public bool AttestationValidated { get; }

        /// <summary>
        /// Certificate chain path from attestation to root (root at end). Null when validation fails.
        /// </summary>
        public List<string>? AttestationPath { get; }

        /// <summary>
        /// Creates a new FIDO2 attestation result.
        /// </summary>
        /// <param name="attestationValidated">Whether validation succeeded.</param>
        /// <param name="attestationPath">Chain path (root subject names). Null when validation fails.</param>
        public FIDO2AttestationResult(bool attestationValidated, List<string>? attestationPath = null)
        {
            AttestationValidated = attestationValidated;
            AttestationPath = attestationPath;
        }
    }
}
