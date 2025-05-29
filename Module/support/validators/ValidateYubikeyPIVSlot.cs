/// <summary>
/// Validates YubiKey PIV slot assignments.
/// Ensures slots are valid and optionally restricts attestation slot usage.
/// 
/// .EXAMPLE
/// [ValidateYubikeyPIVSlot()]
/// [Parameter(Mandatory = true)]
/// public PIVSlot Slot { get; set; }
/// 
/// .EXAMPLE
/// [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
/// [Parameter(Mandatory = true)]
/// public PIVSlot NonAttestationSlot { get; set; }
/// </summary>

// Imports
using powershellYK.PIV;
using System.Management.Automation;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.validators
{
    // Custom validator for PIV slot assignments
    class ValidateYubikeyPIVSlot : ValidateArgumentsAttribute
    {
        // Whether to allow attestation slot usage
        public bool DontAllowAttestion { get; set; } = false;

        // Validate PIV slot assignment
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Check if argument is a PIV slot
            if (arguments is PIVSlot)
            {
                // Validate slot number
                if (!PivSlot.IsValidSlotNumber((PIVSlot)arguments))
                {
                    throw new ArgumentException("Invalid slot");
                }

                // Check attestation slot restriction
                if (DontAllowAttestion && (PIVSlot)arguments == PivSlot.Attestation)
                {
                    throw new ArgumentException("Attestation slot is not allowed");
                }
                return;
            }
            throw new ArgumentException("Invalid type for slot");
        }
    }
}
