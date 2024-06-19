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
                if ((string)inputData == "1" || (string)inputData == "ShortPress" || (string)inputData == "Short" || (string)inputData == "[Yubico.YubiKey.Otp.Slot]::ShortPress")
                {
                    return Yubico.YubiKey.Otp.Slot.ShortPress;
                }
                else if ((string)inputData == "2" || (string)inputData == "LongPress" || (string)inputData == "Long" || (string)inputData == "[Yubico.YubiKey.Otp.Slot]::LongPress")
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
            else if (inputData is Slot)
            {
                return inputData;
            }
            throw new ArgumentException($"Unable to parse slot for input data");
        }
    }
}
