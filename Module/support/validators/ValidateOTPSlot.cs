using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    class ValidateOTPSlot : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {

            if (((PSObject)arguments).BaseObject is Slot)
            {
                return;
            }
            throw new ArgumentException("Invalid formatted slot");
        }
    }
}
