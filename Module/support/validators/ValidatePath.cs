using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    public class ValidatePath : ValidateArgumentsAttribute
    {
        public string? _fileExt { get; } = "";
        public bool _fileMustExist { get; } = false;
        public bool _fileMustNotExist { get; } = false;
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (arguments is System.IO.FileInfo && _fileMustExist && !((FileInfo)arguments!).Exists)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} does not exist.");
            }
            if (arguments is System.IO.FileInfo && _fileMustNotExist && ((FileInfo)arguments!).Exists)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} already exists.");
            }
            if (arguments is System.IO.FileInfo && _fileMustNotExist && ((FileInfo)arguments).Extension == _fileExt)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} does not end in required {_fileExt}.");
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
        public ValidatePath(bool fileMustExist, bool fileMustNotExist, string? fileExt = null)
        {
            this._fileMustExist = fileMustExist;
            this._fileMustNotExist = fileMustNotExist;
            this._fileExt = fileExt;
        }
    }

}
