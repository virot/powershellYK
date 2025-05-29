/// <summary>
/// Provides friendly name generation for YubiKey devices.
/// Determines the appropriate product name based on device capabilities,
/// form factor, and firmware version.
/// 
/// .EXAMPLE
/// $yubiKey = Get-YubiKey
/// $friendlyName = [powershellYK.support.PowershellYKText]::FriendlyName($yubiKey)
/// Write-Host "Device: $friendlyName"
/// 
/// .EXAMPLE
/// Get-YubiKey | ForEach-Object {
///     $name = [powershellYK.support.PowershellYKText]::FriendlyName($_)
///     Write-Host "Found: $name"
/// }
/// </summary>

// Imports
using Yubico.YubiKey;

namespace powershellYK.support
{
    // Utility class for generating YubiKey friendly names
    public static class PowershellYKText
    {
        // Generates a friendly name for a YubiKey device based on its capabilities
        public static string FriendlyName(YubiKeyDevice yubiKeyDevice)
        {
            try
            {
                // Extract device information
                string firmwareVersion = yubiKeyDevice.FirmwareVersion.ToString();
                int family = yubiKeyDevice.FirmwareVersion.Major;
                bool isFips = yubiKeyDevice.IsFipsSeries;
                bool isSky = yubiKeyDevice.IsSkySeries;
                bool isNFC = yubiKeyDevice.AvailableNfcCapabilities != YubiKeyCapabilities.None;
                bool isPINComplexity = yubiKeyDevice.IsPinComplexityEnabled;
                bool isPIV = yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.Piv) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.Piv);
                bool isBio = yubiKeyDevice.FormFactor is FormFactor.UsbABiometricKeychain || yubiKeyDevice.FormFactor is FormFactor.UsbCBiometricKeychain;
                bool isFIDO = yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.Fido2) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.Fido2) || yubiKeyDevice.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.FidoU2f) || yubiKeyDevice.AvailableNfcCapabilities.HasFlag(YubiKeyCapabilities.FidoU2f);
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
                    // Handle 5 Series devices
                    if (isSky)
                    {
                        switch (formFactor)
                        {
                            case FormFactor.UsbAKeychain:
                                return "Security Key A by Yubico";
                            case FormFactor.UsbCKeychain:
                                return "Security Key C by Yubico";
                            default:
                                return "Security Key by Yubico";
                        }
                    }
                    // YubiKey Bio
                    else if (isBio)
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
                // Return unknown model if any errors occur
            }
            return "Unknown YubiKey model";
        }
    }
}
