using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;
using powershellYK.PIV;

namespace powershellYK
{
    public class Attestion
    {
        public bool? AttestionValidated { get; }
        public uint? SerialNumber { get; }
        public FirmwareVersion? FirmwareVersion { get; }
        public PivPinPolicy? PinPolicy { get; }
        public PivTouchPolicy? TouchPolicy { get; }
        public FormFactor? FormFactor { get; }
        public PIVSlot? Slot { get; }
        public PivAlgorithm? Algorithm { get; }
        public bool? isFIPSSeries { get; } = false;
        public bool? isCSPNSeries { get; } = false;
        public bool? AttestionMatchesCSR { get; } = null;

        public string? AttestionDataLocation { get; } = null;

        public Attestion(bool attestionValidated = false, uint? serialnumber = null, FirmwareVersion? firmwareVersion = null, PivPinPolicy? pivPinPolicy = null, PivTouchPolicy? pivTouchPolicy = null, FormFactor? formFactor = null, PIVSlot? pivSlot = null, PivAlgorithm? pivAlgorithm = null, bool? isFIPSSeries = null, bool? isCSPNSeries = null, bool? AttestionMatchesCSR = null, string? attestionDataLocation = null)
        {
            AttestionValidated = attestionValidated;
            this.SerialNumber = serialnumber;
            this.FirmwareVersion = firmwareVersion;
            this.PinPolicy = pivPinPolicy;
            this.TouchPolicy = pivTouchPolicy;
            this.FormFactor = formFactor;
            this.Slot = pivSlot;
            this.Algorithm = pivAlgorithm;
            this.isFIPSSeries = isFIPSSeries;
            this.isCSPNSeries = isCSPNSeries;
            this.AttestionMatchesCSR = AttestionMatchesCSR;
            this.AttestionDataLocation = attestionDataLocation;
        }
    }
}
