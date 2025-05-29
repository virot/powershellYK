/// <summary>
/// Validates YubiKey PIN code length requirements.
/// Ensures PINs meet minimum and maximum length constraints.
/// 
/// .EXAMPLE
/// [ValidateYubikeyPIN(6, 8)]
/// [Parameter(Mandatory = true)]
/// public SecureString PIN { get; set; }
/// 
/// .EXAMPLE
/// $pin = Read-Host -AsSecureString
/// $validator = [ValidateYubikeyPIN]::new(6, 8)
/// $validator.Validate($pin, $null)
/// </summary>

// Imports
using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    // Custom validator for YubiKey PIN requirements
    public class ValidateYubikeyPIN : ValidateArgumentsAttribute
    {
        // Minimum allowed PIN length
        public int MinLength { get; }

        // Maximum allowed PIN length
        public int MaxLength { get; }

        // Validate PIN length requirements
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Check if PIN length is within bounds
            if (((SecureString)arguments).Length >= MinLength && ((SecureString)arguments).Length <= MaxLength)
            {
                return;
            }
            throw new ArgumentException($"PIN code must be between {MinLength} and {MaxLength}.");
        }

        // Constructor to set length constraints
        public ValidateYubikeyPIN(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
