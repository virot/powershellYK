using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{ 
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

}
