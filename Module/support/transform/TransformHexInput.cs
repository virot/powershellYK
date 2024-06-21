using System.Management.Automation;
using Yubico.YubiKey.Piv;


namespace powershellYK.support.transform
{
    class TransformHexInput : System.Management.Automation.ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is byte[])
            {
                return (byte[])inputData;
            }
            else if (inputData is string)
            {
                return HexConverter.StringToByteArray((string)inputData);
            }
            if (inputData is PSObject)
            {
                if (((PSObject)inputData).BaseObject is byte[])
                {
                    return (byte[])((PSObject)inputData).BaseObject;
                }
                else if (((PSObject)inputData).BaseObject is String)
                {
                    return HexConverter.StringToByteArray((string)((PSObject)inputData).BaseObject);
                }
            }
            return inputData;
        }
    }
}
