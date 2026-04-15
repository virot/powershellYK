/// <summary>
/// Creates a pseudo random challenge to support FIDO2 attestation output (among other things).
/// Uses cryptographically secure random data via RandomNumberGenerator.
/// Writes the challenge to a file only; defaults to "challenge.bin" if -OutFile is not specified.
/// Default length is 128 bytes.
/// 
/// .EXAMPLE
/// New-Challenge
/// Creates a 128-byte challenge and writes it to "challenge.bin"
/// 
/// .EXAMPLE
/// New-Challenge -Length 256
/// Creates a 256-byte challenge and writes it to "challenge.bin"
/// 
/// .EXAMPLE
/// New-Challenge -OutFile "MyChallenge.bin"
/// Creates a 128-byte challenge and writes it to "MyChallenge.bin"
/// 
/// .EXAMPLE
/// New-Challenge -OutFile "MyChallenge.bin" -Force
/// Overwrites MyChallenge.bin if it already exists
/// </summary>

// Imports
using System.IO;
using System.Management.Automation;
using System.Security.Cryptography;
using powershellYK.support.transform;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsCommon.New, "Challenge")]
    public class NewChallengeCommand : PSCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Length of the challenge in bytes")]
        [ValidateRange(1, 4096)]
        public int Length { get; set; } = 128;

        [Parameter(Mandatory = false, HelpMessage = "Path for the output file. Defaults to challenge.bin")]
        [TransformPath]
        public FileInfo? OutFile { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Force overwriting existing files.")]
        public SwitchParameter Force { get; set; } = false;

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Determine output path; default to challenge.bin in the current PowerShell location
            if (OutFile is null)
            {
                string pswd = SessionState.Path.CurrentFileSystemLocation.Path;
                OutFile = new FileInfo(Path.Combine(pswd, "challenge.bin"));
            }

            string path = OutFile.FullName;

            // Generate cryptographically secure random challenge bytes
            byte[] challenge = new byte[Length];
            RandomNumberGenerator.Fill(challenge);

            // CreateNew fails atomically if file exists; Create allows overwrite
            using (var fs = new FileStream(path, Force.IsPresent ? FileMode.Create : FileMode.CreateNew))
            {
                fs.Write(challenge, 0, challenge.Length);
            }

            WriteInformation($"Challenge of length {Length} generated and written to file '{path}'.", new[] { "Challenge", "Info" });
        }
    }
}
