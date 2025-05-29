/// <summary>
/// Validates PIV management key format and length requirements.
/// Ensures keys are either 16, 24, or 32 bytes in length.
/// 
/// .EXAMPLE
/// [ValidatePIVManagementKey()]
/// [Parameter(Mandatory = true)]
/// public byte[] ManagementKey { get; set; }
/// 
/// .EXAMPLE
/// [ValidatePIVManagementKey()]
/// [Parameter(Mandatory = true)]
/// public string ManagementKeyHex { get; set; }
/// </summary>

// Imports
using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    // Custom validator for PIV management key requirements
    class ValidatePIVManagementKey : ValidateArgumentsAttribute
    {
        // Validate management key format and length
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Handle byte array input
            if (arguments is byte[])
            {
                throw new ArgumentException("Incorrect number of bytes, should be: 16, 24 or 32 bytes");
            }
            // Handle PSObject-wrapped byte array
            else if (((PSObject)arguments).BaseObject is byte[])
            {
                if (((byte[])(((PSObject)arguments).BaseObject)).Length == 24)
                {
                    return;
                }
                if (((byte[])(((PSObject)arguments).BaseObject)).Length == 32)
                {
                    return;
                }
                if (((byte[])(((PSObject)arguments).BaseObject)).Length == 32)
                {
                    return;
                }
                throw new ArgumentException("Incorrect number of bytes, should be: 16, 24 or 32 bytes");
            }
            // Handle string input
            else if (arguments is string || ((PSObject)arguments).BaseObject is string)
            {
                throw new ArgumentException("Incorrect length of string, should be: 32, 48 or 64 characters");
            }
            throw new ArgumentException("Incorrect type of argument, should be: byte[] or string");
        }
    }
}
