using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


namespace powershellYK.support.transform
{
    class TransformCertificatePath_Certificate : System.Management.Automation.ArgumentTransformationAttribute
    {
        private static string pemFormatRegex = "^-----[^-]*-----(?<certificateContent>.*)-----[^-]*-----$";

        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            
            if (inputData is string)
            {
                Regex regex = new Regex(pemFormatRegex, RegexOptions.Singleline);
                Match? match;
                match = regex.Match((string)inputData);
                if (match.Success)
                {
                    try
                    {
                        byte[] certificateAttBytes = Convert.FromBase64String(match.Groups["certificateContent"].Value);
                        return new X509Certificate2(certificateAttBytes);
                    }
                    catch
                    {
                        throw new ArgumentException($"Invalid certificate data.");
                    }
                }
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
            else if (inputData is System.Security.Cryptography.X509Certificates.X509Certificate2)
            {
                return inputData;
            }
            throw new ArgumentException($"Invalid certificate / certificate path");
        }
    }
}
