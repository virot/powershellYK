using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Yubico.YubiKey.Piv;


namespace powershellYK.support.transform
{
    class TransformCertificateRequest_Path : System.Management.Automation.ArgumentTransformationAttribute
    {
        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is PSObject)
            {
                inputData = ((PSObject)inputData).BaseObject;
            }
                if (inputData is string)
                {
                    Regex regex = new Regex(pemFormatRegex, RegexOptions.Singleline);
                    Match match = regex.Match(inputData.ToString()!);
                    if (match.Success)
                    {
                        return CertificateRequest.LoadSigningRequestPem((string)inputData, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.Default);
                    }
                    else // Check if it is a file or throw an error.
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
                            string CertificateRequestString = System.IO.File.ReadAllText(newPath);
                            return CertificateRequest.LoadSigningRequestPem(CertificateRequestString, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.Default);
                        }
                        else
                        {
                            throw new ArgumentException("String must contain PEM certificate request or path to same.", "CertificateRequest");
                        }
                    }
                }
                else if (inputData is byte[])
                {
                    return CertificateRequest.LoadSigningRequest((byte[])(inputData), HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
                }

            throw new ArgumentException($"Unable to parse CertificateRequest from data.");
        }
    }
}
