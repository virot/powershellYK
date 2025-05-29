/// <summary>
/// Validates file path requirements for YubiKey operations.
/// Checks file existence, non-existence, and extension requirements.
/// 
/// .EXAMPLE
/// [ValidatePath(".pem")]
/// [Parameter(Mandatory = true)]
/// public FileInfo CertificatePath { get; set; }
/// 
/// .EXAMPLE
/// [ValidatePath(true, false, ".key")]
/// [Parameter(Mandatory = true)]
/// public FileInfo KeyPath { get; set; }
/// </summary>

// Imports
using System.Management.Automation;
using System.Security;
using Yubico.YubiKey.Otp;

namespace powershellYK.support.validators
{
    // Custom validator for file path requirements
    public class ValidatePath : ValidateArgumentsAttribute
    {
        // Required file extension
        public string? _fileExt { get; } = "";

        // Whether file must exist
        public bool _fileMustExist { get; } = false;

        // Whether file must not exist
        public bool _fileMustNotExist { get; } = false;

        // Validate file path requirements
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            // Check if file exists when required
            if (arguments is System.IO.FileInfo && _fileMustExist && !((FileInfo)arguments!).Exists)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} does not exist.");
            }

            // Check if file doesn't exist when required
            if (arguments is System.IO.FileInfo && _fileMustNotExist && ((FileInfo)arguments!).Exists)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} already exists.");
            }

            // Check file extension when required
            if (arguments is System.IO.FileInfo && _fileMustNotExist && ((FileInfo)arguments).Extension == _fileExt)
            {
                throw new ArgumentException($"Path {((FileInfo)arguments).FullName} does not end in required {_fileExt}.");
            }
            return;
        }

        // Constructor with file extension requirement
        public ValidatePath(string fileExt)
        {
            this._fileExt = fileExt;
        }

        // Default constructor
        public ValidatePath()
        {
        }

        // Constructor with existence and extension requirements
        public ValidatePath(bool fileMustExist, bool fileMustNotExist, string? fileExt = null)
        {
            this._fileMustExist = fileMustExist;
            this._fileMustNotExist = fileMustNotExist;
            this._fileExt = fileExt;
        }
    }
}
