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
        public bool? AttestionValidated { get; set; }
        public uint? SerialNumber { get; set; }
        public FirmwareVersion? FirmwareVersion { get; set; }
        public PivPinPolicy? PinPolicy { get; set; }
        public PivTouchPolicy? TouchPolicy { get; set; }
        public FormFactor? FormFactor { get; set; }
        public PIVSlot Slot { get; set; }
        public bool? isFIPSSeries { get; set; } = false;
        public bool? isCSPNSeries { get; set; } = false;
        public bool? AttestionMatchesCSR { get; set; } = null;
    }
}
