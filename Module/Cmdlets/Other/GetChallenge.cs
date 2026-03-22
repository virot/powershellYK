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
using System.Management.Automation;
using System.Security.Cryptography;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsCommon.Get, "Challenge")]
    public class GetChallengeCommand : Cmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Length of the challenge in bytes")]
        [ValidateRange(1, 4096)]
        public int Length { get; set; } = 128;

        [Parameter(Mandatory = false, HelpMessage = "Path for the output file. Defaults to challenge.bin")]
        public string? OutFile { get; set; }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Generate cryptographically secure random challenge bytes
            byte[] challenge = new byte[Length];
            RandomNumberGenerator.Fill(challenge);

            // Determine output path; default to challenge.bin when not specified
            string path = string.IsNullOrEmpty(OutFile) ? "challenge.bin" : OutFile;
            System.IO.File.WriteAllBytes(path, challenge);

            // Confirm completion to the user
            WriteInformation($"Challenge of length {Length} generated and written to file '{path}'.", new[] { "Challenge", "Info" });
        }
    }
}
