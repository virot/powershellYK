/// <summary>
/// Transforms hex string input into byte arrays for YubiKey operations.
/// Handles both string and byte array inputs, including PSObject wrappers.
/// 
/// .EXAMPLE
/// [TransformHexInput()]
/// [Parameter(Mandatory = true)]
/// public byte[] Data { get; set; }
/// 
/// .EXAMPLE
/// $hexString = "1A2B3C4D"
/// $bytes = [TransformHexInput]::Transform($null, $hexString)
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey.Piv;

namespace powershellYK.support.transform
{
    // Custom argument transformation for hex string to byte array conversion
    class TransformHexInput : System.Management.Automation.ArgumentTransformationAttribute
    {
        // Transform input data into byte array
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            // Handle byte array input
            if (inputData is byte[])
            {
                return (byte[])inputData;
            }
            // Handle string input
            else if (inputData is string)
            {
                return Converter.StringToByteArray((string)inputData);
            }
            // Handle PSObject wrapper
            if (inputData is PSObject)
            {
                if (((PSObject)inputData).BaseObject is byte[])
                {
                    return (byte[])((PSObject)inputData).BaseObject;
                }
                else if (((PSObject)inputData).BaseObject is String)
                {
                    return Converter.StringToByteArray((string)((PSObject)inputData).BaseObject);
                }
            }
            return inputData;
        }
    }
}
