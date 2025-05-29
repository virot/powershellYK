/// <summary>
/// Validates YubiKey password or passphrase length requirements.
/// Ensures passwords meet minimum and maximum length constraints.
/// 
/// .EXAMPLE
/// [ValidateYubikeyPassword(6, 32)]
/// [Parameter(Mandatory = true)]
/// public SecureString Password { get; set; }
/// 
/// .EXAMPLE
/// $password = Read-Host -AsSecureString
/// $validator = [ValidateYubikeyPassword]::new(6, 32)
/// $validator.Validate($password, $null)
/// </summary>

// Imports
using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    // Custom validator for YubiKey password requirements
    public class ValidateYubikeyPassword : ValidateArgumentsAttribute
    {
        // Minimum allowed password length
        public int MinLength { get; }

        // Maximum allowed password length
        public int MaxLength { get; }

        // Validate password length requirements
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Check if password length is within bounds
            if (((SecureString)arguments).Length >= MinLength && ((SecureString)arguments).Length <= MaxLength)
            {
                return;
            }
            throw new ArgumentException($"Password or passphrase must be between {MinLength} and {MaxLength}.");
        }

        // Constructor to set length constraints
        public ValidateYubikeyPassword(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
