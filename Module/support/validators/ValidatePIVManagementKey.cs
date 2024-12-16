using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    class ValidatePIVManagementKey : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (arguments is byte[])
            {
                throw new ArgumentException("Incorrect number of bytes, should be: 16, 24 or 32 bytes");
            }
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
            else if (arguments is string || ((PSObject)arguments).BaseObject is string)
            {
                throw new ArgumentException("Incorrect length of string, should be: 32, 48 or 64 characters");
            }
            throw new ArgumentException("Incorrect type of argument, should be: byte[] or string");
        }
    }

}
