using System.Management.Automation;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.validators
{
    class ValidateYubikeyPIVSlot : ValidateArgumentsAttribute
    {
        public bool DontAllowAttestion { get; set; } = false;
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {

            if (arguments is byte)
            {
                if (!PivSlot.IsValidSlotNumber((byte)arguments))
                {
                    throw new ArgumentException("Invalid slot");
                }
                if (DontAllowAttestion && (byte)arguments == PivSlot.Attestation)
                {
                    throw new ArgumentException("Attestation slot is not allowed");
                }
                return;
            }
            throw new ArgumentException("Invalid formatted slot");
        }
    }
}
