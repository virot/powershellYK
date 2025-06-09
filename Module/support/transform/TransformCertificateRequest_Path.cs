/// <summary>
/// Transforms certificate request input paths and PEM-formatted requests into CertificateRequest objects.
/// Handles both file paths and PEM-formatted certificate request strings.
/// Supports relative and absolute file paths.
/// 
/// .EXAMPLE
/// [TransformCertificateRequest_Path()]
/// [Parameter(Mandatory = true)]
/// public CertificateRequest CertificateRequest { get; set; }
/// 
/// .EXAMPLE
/// $csr = Get-Item "request.pem" | Select-Object -ExpandProperty FullName
/// $certRequest = [TransformCertificateRequest_Path]::Transform($null, $csr)
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.transform
{
    // Custom argument transformation for certificate request paths and PEM data
    class TransformCertificateRequest_Path : System.Management.Automation.ArgumentTransformationAttribute
    {
        // Regular expression pattern for PEM certificate request format
        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        // Transform input data into CertificateRequest object
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            // Handle PSObject wrapper
            if (inputData is PSObject)
            {
                inputData = ((PSObject)inputData).BaseObject;
            }

            // Process string input
            if (inputData is string)
            {
                // Check for PEM format
                Regex regex = new Regex(pemFormatRegex, RegexOptions.Singleline);
                Match match = regex.Match(inputData.ToString()!);

                if (match.Success)
                {
                    return CertificateRequest.LoadSigningRequestPem((string)inputData, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.Default);
                }
                else
                {
                    // there is a bug with GetCurrentDirectory, which breaks relative paths
                    // Should probably really revert to the incorrect path after this.
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Resolve-Path");
                    myPowersShellInstance.AddParameter("Path", inputData);
                    myPowersShellInstance.AddCommand("Select-Object").AddParameter("ExpandProperty", "Path");
                    System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> returnValue = myPowersShellInstance.Invoke();
                    string? newPath = returnValue.FirstOrDefault()?.BaseObject.ToString();

                    if (System.IO.File.Exists(newPath))
                    {
                        // Load certificate request from file
                        string CertificateRequestString = System.IO.File.ReadAllText(newPath);
                        return CertificateRequest.LoadSigningRequestPem(CertificateRequestString, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.Default);
                    }
                    else
                    {
                        throw new ArgumentException("String must contain PEM certificate request or path to same.", "CertificateRequest");
                    }
                }
            }
            // Process byte array input
            else if (inputData is byte[])
            {
                return CertificateRequest.LoadSigningRequest((byte[])(inputData), HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
            }

            throw new ArgumentException($"Unable to parse certificate request from data.");
        }
    }
}
