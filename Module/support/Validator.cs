using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.Validators
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
    public class ValidateYubikeyPIN : ValidateArgumentsAttribute
    {
        public int MinLength { get; }
        public int MaxLength { get; }
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (((SecureString)arguments).Length >= MinLength && ((SecureString)arguments).Length <= MaxLength)
            {
                return;
            }
            throw new ArgumentException($"PIN code must be between {MinLength} and {MaxLength}.");
        }
        public ValidateYubikeyPIN(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }

    public class ValidateYubikeyPassword : ValidateArgumentsAttribute
    {
        public int MinLength { get; }
        public int MaxLength { get; }
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (((SecureString)arguments).Length >= MinLength && ((SecureString)arguments).Length <= MaxLength)
            {
                return;
            }
            throw new ArgumentException($"Password code must be between {MinLength} and {MaxLength}.");
        }
        public ValidateYubikeyPassword(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }

}
