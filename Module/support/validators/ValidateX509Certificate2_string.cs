/// <summary>
/// Validates X509Certificate2 input for YubiKey operations.
/// Ensures input is either an X509Certificate2 object or a valid certificate string.
/// 
/// .EXAMPLE
/// [ValidateX509Certificate2_string()]
/// [Parameter(Mandatory = true)]
/// public X509Certificate2 Certificate { get; set; }
/// 
/// .EXAMPLE
/// [ValidateX509Certificate2_string()]
/// [Parameter(Mandatory = true)]
/// public string CertificatePath { get; set; }
/// </summary>

// Imports
using System.Management.Automation;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    // Custom validator for X509Certificate2 input
    class ValidateX509Certificate2_string : ValidateArgumentsAttribute
    {
        // Validate certificate input type
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Handle direct X509Certificate2 input
            if (arguments is X509Certificate2)
            {
                return;
            }
            // Handle PSObject-wrapped input
            else if (arguments is PSObject)
            {
                if (((PSObject)arguments).BaseObject is X509Certificate2)
                {
                    return;
                }
                throw new ArgumentException($"Incorrect type '{((PSObject)arguments).BaseObject.GetType()}', should be: X509Certificate2 or string");
            }
            throw new ArgumentException($"Incorrect type '{arguments.GetType()}', should be: X509Certificate2 or string");
        }
    }
}
