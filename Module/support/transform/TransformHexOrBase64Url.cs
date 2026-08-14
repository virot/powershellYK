/// <summary>
/// Transforms hex or base64url string input into byte arrays.
/// Used for previewSign KeyHandle and PublicKey values copied from Format-List.
///
/// .EXAMPLE
/// [TransformHexOrBase64Url()]
/// [Parameter(Mandatory = true)]
/// public byte[] KeyHandle { get; set; }
///
/// .EXAMPLE
/// $bytes = [powershellYK.support.Converter]::HexOrBase64UrlToByteArray("z3FfSyAL5pLzfJ_cfM6_ZKt13jxJ1qkbdJqX3sbRJAwASA")
/// </summary>

// Imports
using System.Management.Automation;

namespace powershellYK.support.transform
{
    // Custom argument transformation for hex or base64url string to byte array
    class TransformHexOrBase64Url : System.Management.Automation.ArgumentTransformationAttribute
    {
        // Transform input data into byte array
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is byte[] bytes)
            {
                return bytes;
            }
            if (inputData is string text)
            {
                return Converter.HexOrBase64UrlToByteArray(text);
            }
            if (inputData is PSObject psObject)
            {
                if (psObject.BaseObject is byte[] wrappedBytes)
                {
                    return wrappedBytes;
                }
                if (psObject.BaseObject is string wrappedText)
                {
                    return Converter.HexOrBase64UrlToByteArray(wrappedText);
                }
            }
            return inputData;
        }
    }
}
