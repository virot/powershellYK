﻿using Yubico.YubiKey;

namespace powershellYK.support
{
    public static class PowershellYKText
    {
        public static string FriendlyName(YubiKeyDevice yubiKeyDevice)
        {
            try
            {
                // Use the supplied YubiKeyDevice object to get the information needed.
                // This should be generated by the SDK
                string firmwareVersion = yubiKeyDevice.FirmwareVersion.ToString();
                int family = yubiKeyDevice.FirmwareVersion.Major;
                bool isFips = yubiKeyDevice.IsFipsSeries;
                bool isSky = yubiKeyDevice.IsSkySeries;
                bool isNFC = yubiKeyDevice.AvailableNfcCapabilities != YubiKeyCapabilities.None;
                bool isPINComplexity = yubiKeyDevice.IsPinComplexityEnabled;
                bool isPIV = yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.Piv) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.Piv);
                bool isBio = yubiKeyDevice.FormFactor is FormFactor.UsbABiometricKeychain || yubiKeyDevice.FormFactor is FormFactor.UsbCBiometricKeychain;
                bool isFIDO = yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.Fido2) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.Fido2) || yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.FidoU2f) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.FidoU2f) ;
                FormFactor formFactor = yubiKeyDevice.FormFactor;

                // Use the information to generate a friendly name

                if (family == 2 || family == 3) { return "YubiKey Standard"; };
                if (family == 4)
                {
                    if (isFips)
                    {
                        if (formFactor == FormFactor.UsbAKeychain) { return "YubiKey FIPS (4 Series)"; };
                        if (formFactor == FormFactor.UsbANano) { return "YubiKey Nano FIPS (4 Series)"; };
                        if (formFactor == FormFactor.UsbCKeychain) { return "YubiKey C FIPS (4 Series)"; };
                        if (formFactor == FormFactor.UsbCNano) { return "YubiKey C Nano FIPS (4 Series)"; };
                    }
                    else
                    {
                        if (formFactor == FormFactor.UsbAKeychain) { return "YubiKey (4 Series)"; };
                        if (formFactor == FormFactor.UsbANano) { return "YubiKey Nano (4 Series)"; };
                        if (formFactor == FormFactor.UsbCKeychain) { return "YubiKey C (4 Series)"; };
                        if (formFactor == FormFactor.UsbCNano) { return "YubiKey C Nano (4 Series)"; };
                    }
                }
                if (family == 5)
                {
                    // YubiKey Bio
                    if (isBio)
                    {
                        if (isPIV) // Multi-Protocol Edition (AKA "MPE")
                        {
                            if (formFactor == FormFactor.UsbABiometricKeychain) { return "YubiKey Bio - Multi-Protocol Edition"; };
                            if (formFactor == FormFactor.UsbCBiometricKeychain) { return "YubiKey C Bio - Multi-Protocol Edition"; };
                        }
                        else // FIDO Edition
                        {
                            if (formFactor == FormFactor.UsbABiometricKeychain) { return "YubiKey Bio - FIDO Edition"; };
                            if (formFactor == FormFactor.UsbCBiometricKeychain) { return "YubiKey C Bio - FIDO Edition"; };
                        }
                    }
                    else if (isFips) // YubiKey 5 Series (FIPS)
                    {
                        if (formFactor == FormFactor.UsbAKeychain) { return "YubiKey 5 NFC FIPS"; };
                        if (formFactor == FormFactor.UsbANano) { return "YubiKey 5 Nano FIPS"; };
                        if (formFactor == FormFactor.UsbCKeychain) { return "YubiKey 5C FIPS"; };
                        if (formFactor == FormFactor.UsbCNano) { return "YubiKey 5C Nano FIPS"; };
                        if (formFactor == FormFactor.UsbCLightning) { return "YubiKey 5Ci FIPS"; };
                    }
                    else if (isNFC) // YubiKey 5 Series (standard)
                    {
                        if (formFactor == FormFactor.UsbAKeychain) { return "YubiKey 5 NFC"; };
                        if (formFactor == FormFactor.UsbCKeychain) { return "YubiKey 5C NFC"; };
                    }
                    else
                    {
                        if (formFactor == FormFactor.UsbAKeychain) { return "YubiKey 5"; };
                        if (formFactor == FormFactor.UsbANano) { return "YubiKey 5 Nano"; };
                        if (formFactor == FormFactor.UsbCKeychain) { return "YubiKey 5C"; };
                        if (formFactor == FormFactor.UsbCNano) { return "YubiKey 5C Nano"; };
                        if (formFactor == FormFactor.UsbCLightning) { return "YubiKey 5Ci"; };
                    }
                }

            }
            catch
            {
            }
            return "Unknown YubiKey model";
        }
    }
}
