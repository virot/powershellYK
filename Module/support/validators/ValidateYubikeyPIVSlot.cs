using powershellYK.PIV;
using System.Management.Automation;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.validators
{
    class ValidateYubikeyPIVSlot : ValidateArgumentsAttribute
    {
        public bool DontAllowAttestion { get; set; } = false;
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {

            if (arguments is PIVSlot)
            {
                if (!PivSlot.IsValidSlotNumber((PIVSlot)arguments))
                {
                    throw new ArgumentException("Invalid slot");
                }
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
