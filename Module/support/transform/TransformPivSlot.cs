using System.Management.Automation;
using Yubico.YubiKey.Piv;


namespace powershellYK.support.transform
{
    class TransformPivSlot : System.Management.Automation.ArgumentTransformationAttribute
    {
        private static byte[] _pivSlots = new byte[] { 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x9A, 0x9C, 0x9D, 0x9E, 0x9F };
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is string)
            {
                if (String.Equals((string)inputData, "PIV Authentication", StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, "Authentication", StringComparison.OrdinalIgnoreCase))
                {
                    return (byte)0x9a;
                }
                else if (String.Equals((string)inputData, "Digital Signature", StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, "Signature", StringComparison.OrdinalIgnoreCase))
                {
                    return (byte)0x9c;
                }
                else if (String.Equals((string)inputData, "Key Management", StringComparison.OrdinalIgnoreCase))
                {
                    return (byte)0x9d;
                }
                else if (String.Equals((string)inputData, "Card Authentication", StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, "Card", StringComparison.OrdinalIgnoreCase))
                {
                    return (byte)0x9e;
                }
                else
                {
                    foreach (byte slot in _pivSlots)
                    {
                        if (String.Equals((string)inputData, slot.ToString(), StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, $"0x{slot:X2}", StringComparison.OrdinalIgnoreCase))
                        {
                            return slot;
                        }
                    }
                }
            }
            else if (inputData is int)
            {
                if ((int)inputData == 0x9a || ((int)inputData == 0x9c && (int)inputData <= 0x9f) || ((int)inputData >= 0x82 && (int)inputData <= 0x95))
                {
                    return (byte)(int)inputData;
                }
            }
            else if (inputData is byte)
            {
                if ((byte)inputData == 0x9a || ((byte)inputData <= 0x9c && (byte)inputData <= 0x9f) || ((byte)inputData >= 0x82 && (byte)inputData <= 0x95))
                {
                    return (byte)inputData;
                }
            }
            else if (inputData is PSObject)
            {
                if (((PSObject)inputData).BaseObject is Int32)
                {

                    foreach (byte slot in _pivSlots)
                    {
                        if ((Int32)(((PSObject)inputData).BaseObject) == slot)
                        {
                            return slot;
                        }
                    }
                }
            }
            throw new ArgumentException($"Unable to parse slot for input data");
        }
    }
}
