using System.Management.Automation;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    class ValidateX509Certificate2_string : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (arguments is X509Certificate2)
            {
                return;
            }
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
