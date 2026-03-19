/// <summary>
/// Creates a pseudo random challenge to support FIDO2 attestation output (among other things).
/// Uses cryptographically secure random data via RandomNumberGenerator.
/// Writes the challenge to a file only; defaults to "challenge.bin" if -OutFile is not specified.
/// 
/// .EXAMPLE
/// Get-Challenge
/// Creates a 128-byte challenge and writes it to "challenge.bin"
/// 
/// .EXAMPLE
/// Get-Challenge -Length 256
/// Creates a 256-byte challenge and writes it to "challenge.bin"
/// 
/// .EXAMPLE
/// Get-Challenge -OutFile "MyChallenge.bin"
/// Creates a 128-byte challenge and writes it to "MyChallenge.bin"
/// </summary>

// Imports
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.IO;
using System.Management.Automation;
using System.Security.Cryptography;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsCommon.New, "Challenge")]
    public class NewChallengeCommand : Cmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Length of the challenge in bytes")]
        [ValidateRange(1, 4096)]
        public int Length { get; set; } = 128;

        [TransformPath]
        [Parameter(Mandatory = true, HelpMessage = "Path for the output file.")]
        public required System.IO.FileInfo OutFile { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Force overwriting existing files.")]
        public SwitchParameter Force { get; set; } = false;

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            if (OutFile.Exists && !Force.IsPresent)
            {
                var ex = new IOException($"File already exists: {OutFile.FullName}");
                ex.HResult = unchecked((int)0x80070050); // ERROR_FILE_EXISTS
                throw ex;
            }

            // Generate cryptographically secure random challenge bytes
            byte[] challenge = new byte[Length];
            RandomNumberGenerator.Fill(challenge);

            // Determine output path; default to challenge.bin when not specified
            using (var file = OutFile.OpenWrite())
            {
                file.Write(challenge);
            }
            // Confirm completion to the user
            WriteInformation($"Challenge of length {Length} generated and written to file '{OutFile.FullName}'.", new[] { "Challenge", "Info" });
        }
    }
}
