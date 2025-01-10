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
                // fix relative paths
                if (! Path.IsPathFullyQualified(inputData?.ToString() ?? ""))
                {
                    var sessionState = engineIntrinsics.SessionState;
                    return Path.Combine(sessionState.Path.CurrentFileSystemLocation.ToString(), inputData?.ToString() ?? "");
                }                
            }
            return inputData!;
        }

        public TransformPath()
        {
        }
    }
}
