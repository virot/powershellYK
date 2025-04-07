using Yubico.YubiKey.Piv;
using Yubico.YubiKey;
using powershellYK.PIV;

namespace powershellYK
{
    public class Attestation
    {
        public bool? AttestationValidated { get; }
        public uint? SerialNumber { get; }
        public FirmwareVersion? FirmwareVersion { get; }
        public PivPinPolicy? PinPolicy { get; }
        public PivTouchPolicy? TouchPolicy { get; }
        public FormFactor? FormFactor { get; }
        public PIVSlot? Slot { get; }
        public PivAlgorithm? Algorithm { get; }
        public bool? isFIPSSeries { get; } = false;
        public bool? isCSPNSeries { get; } = false;
        public bool? AttestationMatchesCSR { get; } = null;

        public string? AttestationDataLocation { get; } = null;
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
