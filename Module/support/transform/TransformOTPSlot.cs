using System.Management.Automation;
using Yubico.YubiKey.Otp;


namespace powershellYK.support.transform
{
    class TransformOTPSlot : System.Management.Automation.ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is string)
            {
                if ((string)inputData == "1" || String.Equals((string)inputData, "ShortPress", StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, "Short", StringComparison.OrdinalIgnoreCase) || (string)inputData == "[Yubico.YubiKey.Otp.Slot]::ShortPress")
                {
                    return Yubico.YubiKey.Otp.Slot.ShortPress;
                }
                else if ((string)inputData == "2" || String.Equals((string)inputData, "LongPress", StringComparison.OrdinalIgnoreCase) || String.Equals((string)inputData, "Long", StringComparison.OrdinalIgnoreCase) || (string)inputData == "[Yubico.YubiKey.Otp.Slot]::LongPress")
                {
                    return Yubico.YubiKey.Otp.Slot.LongPress;
                }
            }
            else if (inputData is int)
            {
                if ((int)inputData == 1)
                {
                    return Yubico.YubiKey.Otp.Slot.ShortPress;
                }
                else if ((int)inputData == 2)
                {
                    return Yubico.YubiKey.Otp.Slot.LongPress;
                }
            }
            return inputData;
        }
    }
}
