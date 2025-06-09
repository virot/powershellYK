/// <summary>
/// Represents information about a PIV slot and its contents.
/// Provides access to slot configuration, key status, and associated certificate.
/// 
/// .EXAMPLE
/// # Create slot information
/// $slotInfo = [powershellYK.PIV.SlotInfo]::new(
///     $slot,
///     $keyStatus,
///     $algorithm,
///     $pinPolicy,
///     $touchPolicy,
///     $certificate,
///     $publicKey
/// )
/// Write-Host "Slot: $($slotInfo.Slot)"
/// 
/// .EXAMPLE
/// # Check key status
/// if ($slotInfo.KeyStatus -eq [Yubico.YubiKey.Piv.PivKeyStatus]::Generated) {
///     Write-Host "Key is generated"
/// }
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Cryptography;

namespace powershellYK.PIV
{
    // Represents information about a PIV slot and its contents
    public class SlotInfo
    {
        // PIV slot identifier
        public PIVSlot Slot { get; private set; }

        // Status of the key in the slot
        public PivKeyStatus? KeyStatus { get; private set; }

        // Algorithm used by the key
        public PivAlgorithm? Algorithm { get; private set; }

        // PIN policy for the slot
        public PivPinPolicy? PinPolicy { get; private set; }

        // Touch policy for the slot
        public PivTouchPolicy? TouchPolicy { get; private set; }

        // Certificate stored in the slot
        public X509Certificate2? Certificate { get; private set; }

        // Public key associated with the slot
        public IPublicKey? PublicKey { get; private set; }

        // Creates a new slot information instance
        public SlotInfo(
            int Slot,
            PivKeyStatus? KeyStatus,
            PivAlgorithm? Algorithm,
            PivPinPolicy? PinPolicy,
            PivTouchPolicy? TouchPolicy,
            X509Certificate2? Certificate,
            IPublicKey? PublicKey = null)
        {
            this.Slot = Slot;
            this.KeyStatus = KeyStatus;
            this.Algorithm = Algorithm;
            this.PinPolicy = PinPolicy;
            this.TouchPolicy = TouchPolicy;
            this.Certificate = Certificate;
            this.PublicKey = PublicKey;
        }
    }
}
