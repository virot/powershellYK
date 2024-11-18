using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;

namespace powershellYK.PIV
{
    public class SlotInfo
    {
        [Hidden]
        public int SlotID { get; private set; }
        public string Slot { get { return $"0x{this.SlotID.ToString("X")}"; }}
        public PivKeyStatus? KeyStatus { get; private set; }
        public PivAlgorithm? Algorithm { get; private set; }
        public PivPinPolicy? PinPolicy { get; private set; }
        public PivTouchPolicy? TouchPolicy { get; private set; }
        public X509Certificate2? Certificate { get; private set; }
        public AsymmetricAlgorithm? PublicKey { get; private set; }

        public SlotInfo(
            int Slot,
            PivKeyStatus? KeyStatus,
            PivAlgorithm? Algorithm,
            PivPinPolicy? PinPolicy,
            PivTouchPolicy? TouchPolicy,
            X509Certificate2? Certificate,
            AsymmetricAlgorithm? PublicKey)
        {
            this.SlotID = Slot;
            this.KeyStatus = KeyStatus;
            this.Algorithm = Algorithm;
            this.PinPolicy = PinPolicy;
            this.TouchPolicy = TouchPolicy;
            this.Certificate = Certificate;
            this.PublicKey = PublicKey;
        }
    }
}
