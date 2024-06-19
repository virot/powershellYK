using System.Management.Automation;
using Yubico.YubiKey.Piv;


namespace powershellYK.support.transform
{
    class TransformPivManagementKey : System.Management.Automation.ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is byte[])
            {
                if (((byte[])inputData).Length == 24)
                {
                    return (byte[])inputData;
                }
                if (((byte[])inputData).Length == 16)
                {
                    return (byte[])inputData;
                }
                if (((byte[])inputData).Length == 32)
                {
                    return (byte[])inputData;
                }
                //throw new ArgumentException("Incorrect number of bytes, should be 16, 24 or 32 bytes");
            }
            else if (inputData is string)
            {
                if (((string)inputData).Length == 48)
                {
                    return HexConverter.StringToByteArray((string)inputData);
                }
                if (((string)inputData).Length == 32)
                {
                    return HexConverter.StringToByteArray((string)inputData);
                }
                if (((string)inputData).Length == 64)
                {
                    return HexConverter.StringToByteArray((string)inputData);
                }
                //throw new ArgumentException("The string length should be, should be 32, 48 or 64 bytes");
            }
            if (inputData is PSObject)
            {
                if (((PSObject)inputData).BaseObject is byte[])
                {
                    if (((byte[])((PSObject)inputData).BaseObject).Length == 24)
                    {
                        return (byte[])((PSObject)inputData).BaseObject;
                    }
                    if (((byte[])((PSObject)inputData).BaseObject).Length == 16)
                    {
                        return (byte[])((PSObject)inputData).BaseObject;
                    }
                    if (((byte[])((PSObject)inputData).BaseObject).Length == 32)
                    {
                        return (byte[])((PSObject)inputData).BaseObject;
                    }
                    //throw new ArgumentException("Incorrect number of bytes, should be 16, 24 or 32 bytes");
                }
                else if (((PSObject)inputData).BaseObject is String)
                {
                    if (((string)((PSObject)inputData).BaseObject).Length == 48)
                    {
                        return HexConverter.StringToByteArray((string)((PSObject)inputData).BaseObject);
                    }
                    if (((string)((PSObject)inputData).BaseObject).Length == 32)
                    {
                        return HexConverter.StringToByteArray((string)((PSObject)inputData).BaseObject);
                    }
                    if (((string)((PSObject)inputData).BaseObject).Length == 64)
                    {
                        return HexConverter.StringToByteArray((string)((PSObject)inputData).BaseObject);
                    }
                    //throw new ArgumentException("The string length should be, should be 32, 48 or 64 bytes");
                }
            }
            return inputData;
        }
    }
}
