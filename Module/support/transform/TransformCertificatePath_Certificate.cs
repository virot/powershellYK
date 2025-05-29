/// <summary>
/// Transforms certificate input paths and PEM-formatted certificates into X509Certificate2 objects.
/// Handles both file paths and PEM-formatted certificate strings.
/// Supports relative and absolute file paths.
/// 
/// .EXAMPLE
/// [TransformCertificatePath_Certificate()]
/// [Parameter(Mandatory = true)]
/// public X509Certificate2 Certificate { get; set; }
/// 
/// .EXAMPLE
/// $cert = Get-Item "cert.pem" | Select-Object -ExpandProperty FullName
/// $certificate = [TransformCertificatePath_Certificate]::Transform($null, $cert)
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace powershellYK.support.transform
{
    // Custom argument transformation for certificate paths and PEM data
    class TransformCertificatePath_Certificate : System.Management.Automation.ArgumentTransformationAttribute
    {
        // Regular expression pattern for PEM certificate format
        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        // Transform input data into X509Certificate2 object
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
                Match? match;
                match = regex.Match((string)inputData);
                if (match.Success)
                {
                    try
                    {
                        // Convert PEM content to certificate
                        byte[] certificateAttBytes = Convert.FromBase64String(match.Groups["certificateContent"].Value);
                        return new X509Certificate2(certificateAttBytes);
                    }
                    catch
                    {
                        throw new ArgumentException($"Invalid certificate data.");
                    }
                }
                // Handle absolute file path
                else if (System.IO.Path.IsPathFullyQualified((string)inputData))
                {
                    if (System.IO.File.Exists((string)inputData))
                    {
                        try
                        {
                            return new X509Certificate2((string)inputData);
                        }
                        catch
                        {
                            throw new ArgumentException($"Invalid certificate: {(string)inputData}");
                        }
                    }
                }
                // Handle relative file path
                else
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Resolve-Path");
                    myPowersShellInstance.AddParameter("Path", inputData);
                    myPowersShellInstance.AddCommand("Select-Object").AddParameter("ExpandProperty", "Path");
                    System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> returnValue = myPowersShellInstance.Invoke();
                    string? newPath = returnValue.FirstOrDefault()?.BaseObject.ToString();

                    if (System.IO.File.Exists(newPath))
                    {
                        try
                        {
                            return new X509Certificate2(newPath);
                        }
                        catch
                        {
                            throw new ArgumentException($"Invalid certificate: {newPath}");
                        }
                    }
                }
            }
            return inputData;
        }
    }
}
