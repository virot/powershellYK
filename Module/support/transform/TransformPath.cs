/// <summary>
/// Transforms relative file paths into fully qualified paths for YubiKey operations.
/// Uses the current PowerShell session location to resolve relative paths.
/// 
/// .EXAMPLE
/// [TransformPath()]
/// [Parameter(Mandatory = true)]
/// public FileInfo Path { get; set; }
/// 
/// .EXAMPLE
/// $relativePath = "certificates\key.pem"
/// $fullPath = [TransformPath]::Transform($null, $relativePath)
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.transform
{
    // Custom argument transformation for file path resolution
    class TransformPath : System.Management.Automation.ArgumentTransformationAttribute
    {
        // Transform input data into fully qualified path
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            // Handle string input
            if (inputData is String)
            {
                // Resolve relative paths
                if (!Path.IsPathFullyQualified(((String)inputData).ToString() ?? ""))
                {
                    var sessionState = engineIntrinsics.SessionState;
                    return new FileInfo(Path.Combine(sessionState.Path.CurrentFileSystemLocation.ToString(), ((String)inputData)?.ToString() ?? ""));
                }
            }
            return inputData!;
        }

        // Default constructor
        public TransformPath()
        {
        }
    }
}
