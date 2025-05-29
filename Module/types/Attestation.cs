/// <summary>
/// Represents attestation data for YubiKey PIV operations.
/// Contains information about the YubiKey's configuration and validation status.
/// 
/// .EXAMPLE
/// # Create attestation data
/// $attestation = [powershellYK.Attestation]::new(
///     $true,  # attestationValidated
///     123456, # serialNumber
///     $firmwareVersion,
///     $pinPolicy,
///     $touchPolicy,
///     $formFactor,
///     $slot,
///     $algorithm
/// )
/// 
/// .EXAMPLE
/// # Check attestation validation status
/// if ($attestation.AttestationValidated) {
///     Write-Host "Attestation is valid"
/// }
/// </summary>

// Imports
using Yubico.YubiKey.Piv;
using Yubico.YubiKey;
using powershellYK.PIV;

namespace powershellYK
{
    // Represents attestation data for YubiKey PIV operations
    public class Attestation
    {
        // Validation status of the attestation
        public bool? AttestationValidated { get; }

        // YubiKey serial number
        public uint? SerialNumber { get; }

        // YubiKey firmware version
        public FirmwareVersion? FirmwareVersion { get; }

        // PIN policy configuration
        public PivPinPolicy? PinPolicy { get; }

        // Touch policy configuration
        public PivTouchPolicy? TouchPolicy { get; }

        // YubiKey form factor
        public FormFactor? FormFactor { get; }

        // PIV slot used
        public PIVSlot? Slot { get; }

        // Algorithm used for the operation
        public PivAlgorithm? Algorithm { get; }

        // Indicates if the YubiKey is FIPS series
        public bool? isFIPSSeries { get; } = false;

        // Indicates if the YubiKey is CSPN series
        public bool? isCSPNSeries { get; } = false;

        // Indicates if attestation matches CSR
        public bool? AttestationMatchesCSR { get; } = null;

        // Location of attestation data
        public string? AttestationDataLocation { get; } = null;

        // Path to attestation data
        public List<string>? AttestationPath { get; } = null;

        public Attestation(bool attestationValidated = false, uint? serialnumber = null, FirmwareVersion? firmwareVersion = null, PivPinPolicy? pivPinPolicy = null, PivTouchPolicy? pivTouchPolicy = null, FormFactor? formFactor = null, PIVSlot? pivSlot = null, PivAlgorithm? pivAlgorithm = null, bool? isFIPSSeries = null, bool? isCSPNSeries = null, bool? AttestationMatchesCSR = null, string? attestationDataLocation = null, List<string>? attestationPath = null)
        {
            AttestationValidated = attestationValidated;
            this.SerialNumber = serialnumber;
            this.FirmwareVersion = firmwareVersion;
            this.PinPolicy = pivPinPolicy;
            this.TouchPolicy = pivTouchPolicy;
            this.FormFactor = formFactor;
            this.Slot = pivSlot;
            this.Algorithm = pivAlgorithm;
            this.isFIPSSeries = isFIPSSeries;
            this.isCSPNSeries = isCSPNSeries;
            this.AttestationMatchesCSR = AttestationMatchesCSR;
            this.AttestationDataLocation = attestationDataLocation;
            this.AttestationPath = attestationPath;
        }
    }
}
