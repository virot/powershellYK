using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;

namespace powershellYK.PIV
{
    public class SlotInfo
    {
        public int? Slot { get; set; }
        public PivKeyStatus? KeyStatus { get; set; }
        public PivAlgorithm? Algorithm { get; set; }
        public PivPinPolicy? PinPolicy { get; set; }
        public PivTouchPolicy? TouchPolicy { get; set; }
        public X509Certificate2? Certificate { get; set; }
        public AsymmetricAlgorithm? PublicKey { get; set; }

    }
}
