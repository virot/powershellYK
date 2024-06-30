using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    public class ValidatePath : ValidateArgumentsAttribute
    {
        public string _fileExt { get; } = "";
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (!Path.Exists((string)arguments))
            {
                throw new ArgumentException($"Path {arguments} does not exist.");
            }
            if (!((string)arguments).EndsWith(_fileExt))
            {
                throw new ArgumentException($"Path {arguments} needs to end with {_fileExt}.");
            }
            return;
        }
        public ValidatePath(string fileExt)
        {
            this._fileExt = fileExt;
        }
        public ValidatePath()
        {
        }
    }

}
