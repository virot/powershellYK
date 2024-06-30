using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Yubico.YubiKey.Piv;


namespace powershellYK.support.transform
{
    class TransformPath : System.Management.Automation.ArgumentTransformationAttribute
    {

        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is string)
            {
                // there is a bug with GetCurrentDirectory, which breaks relative paths
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Resolve-Path");
                myPowersShellInstance.AddParameter("Path", inputData);
                myPowersShellInstance.AddCommand("Select-Object").AddParameter("ExpandProperty", "Path");
                System.Collections.ObjectModel.Collection<System.Management.Automation.PSObject> returnValue = myPowersShellInstance.Invoke();
                string? newPath = returnValue.FirstOrDefault()?.BaseObject.ToString();
                if (System.IO.File.Exists(newPath))
                {
                    return newPath;
                }
            }
            return inputData;
        }
    }
}
