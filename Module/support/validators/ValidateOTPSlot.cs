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
            else if ((((PSObject)arguments).BaseObject is int) || (((PSObject)arguments).BaseObject is byte))
            {
                if ((int)((PSObject)arguments).BaseObject == 1 || (int)((PSObject)arguments).BaseObject == 2)
                {
                    return;
                }
            }
            throw new Exception("Invalid Slot");
        }
    }
}
