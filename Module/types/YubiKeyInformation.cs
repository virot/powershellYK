/// <summary>
/// Represents comprehensive information about a YubiKey device.
/// Provides access to device capabilities, configuration, and status.
/// 
/// .EXAMPLE
/// # Get YubiKey information
/// $info = [powershellYK.YubiKey.YubikeyInformation]::new($yubiKey)
/// Write-Host "Device: $($info.PrettyName)"
/// Write-Host "Serial: $($info.SerialNumber)"
/// 
/// .EXAMPLE
/// # Check device capabilities
/// if ($info.AvailableUsbCapabilities.HasFlag([Yubico.YubiKey.YubiKeyCapabilities]::Fido2)) {
///     Write-Host "Device supports FIDO2"
/// }
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.PinProtocols;
using Yubico.YubiKey.Fido2.Cose;
using powershellYK.support;
using Yubico.YubiKey;

namespace powershellYK.YubiKey
{
    // Represents comprehensive information about a YubiKey
    public class YubikeyInformation
    {
        // Internal YubiKey (hidden from PowerShell)
        [Hidden]
        public YubiKeyDevice YubiKeyDevice { get; }

        // Friendly name for the device
        public string PrettyName { get; }

        // Available USB capabilities
        public YubiKeyCapabilities AvailableUsbCapabilities { get { return YubiKeyDevice.AvailableUsbCapabilities; } }

        // Enabled USB capabilities
        public YubiKeyCapabilities EnabledUsbCapabilities { get { return YubiKeyDevice.EnabledUsbCapabilities; } }

        // Available NFC capabilities
        public YubiKeyCapabilities AvailableNfcCapabilities { get { return YubiKeyDevice.AvailableNfcCapabilities; } }

        // Enabled NFC capabilities
        public YubiKeyCapabilities EnabledNfcCapabilities { get { return YubiKeyDevice.EnabledNfcCapabilities; } }

        // FIPS approval status
        public YubiKeyCapabilities FipsApproved { get { return YubiKeyDevice.FipsApproved; } }

        // FIPS capability status
        public YubiKeyCapabilities FipsCapable { get { return YubiKeyDevice.FipsCapable; } }

        // Reset blocked status
        public YubiKeyCapabilities ResetBlocked { get { return YubiKeyDevice.ResetBlocked; } }

        // NFC restriction status
        public bool IsNfcRestricted { get { return YubiKeyDevice.IsNfcRestricted; } }

        // YubiKey part number
        public string? PartNumber { get { return YubiKeyDevice.PartNumber; } }

        // PIN complexity status
        public bool IsPinComplexityEnabled { get { return YubiKeyDevice.IsPinComplexityEnabled; } }

        // YubiKey serial number
        public int? SerialNumber { get { return YubiKeyDevice.SerialNumber; } }

        // FIPS series status
        public bool IsFipsSeries { get { return YubiKeyDevice.IsFipsSeries; } }

        // SKY series status
        public bool IsSkySeries { get { return YubiKeyDevice.IsSkySeries; } }

        // YubiKey form factor
        public FormFactor FormFactor { get { return YubiKeyDevice.FormFactor; } }

        // YubiKey firmware version
        public FirmwareVersion FirmwareVersion { get { return YubiKeyDevice.FirmwareVersion; } }

        // Template storage version
        public TemplateStorageVersion? TemplateStorageVersion { get { return YubiKeyDevice.TemplateStorageVersion; } }

        // Image processor version
        public ImageProcessorVersion? ImageProcessorVersion { get { return YubiKeyDevice.ImageProcessorVersion; } }

        // Auto-eject timeout
        public int AutoEjectTimeout { get { return YubiKeyDevice.AutoEjectTimeout; } }

        // Challenge-response timeout
        public byte ChallengeResponseTimeout { get { return YubiKeyDevice.ChallengeResponseTimeout; } }

        // YubiKey flags
        public DeviceFlags DeviceFlags { get { return YubiKeyDevice.DeviceFlags; } }

        // Configuration lock status
        public bool ConfigurationLocked { get { return YubiKeyDevice.ConfigurationLocked; } }

        // Available transports
        public Transport AvailableTransports { get { return YubiKeyDevice.AvailableTransports; } }

        // Creates a new YubiKey information instance
        public YubikeyInformation(YubiKeyDevice yubiKey)
        {
            this.YubiKeyDevice = yubiKey;
            this.PrettyName = PowershellYKText.FriendlyName(yubiKey);
        }
    }
}
