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
    public class YubikeyInformation
    {
        [Hidden]
        public YubiKeyDevice YubiKeyDevice { get; }
        public string PrettyName {get;}
        public YubiKeyCapabilities AvailableUsbCapabilities { get { return YubiKeyDevice.AvailableUsbCapabilities; } }
        public YubiKeyCapabilities EnabledUsbCapabilities { get { return YubiKeyDevice.EnabledUsbCapabilities; } }
        public YubiKeyCapabilities AvailableNfcCapabilities { get { return YubiKeyDevice.AvailableNfcCapabilities; } }
        public YubiKeyCapabilities EnabledNfcCapabilities { get { return YubiKeyDevice.EnabledNfcCapabilities; } }
        public YubiKeyCapabilities FipsApproved { get { return YubiKeyDevice.FipsApproved; } }
        public YubiKeyCapabilities FipsCapable { get { return YubiKeyDevice.FipsCapable; } }
        public YubiKeyCapabilities ResetBlocked { get { return YubiKeyDevice.ResetBlocked; } }
        public bool IsNfcRestricted { get { return YubiKeyDevice.IsNfcRestricted; } }
        public string? PartNumber { get { return YubiKeyDevice.PartNumber; } }
        public bool IsPinComplexityEnabled { get { return YubiKeyDevice.IsPinComplexityEnabled; } }
        public int? SerialNumber { get { return YubiKeyDevice.SerialNumber; } }
        public bool IsFipsSeries { get { return YubiKeyDevice.IsFipsSeries; } }
        public bool IsSkySeries { get { return YubiKeyDevice.IsSkySeries; } }
        public FormFactor FormFactor { get { return YubiKeyDevice.FormFactor; } }
        public FirmwareVersion FirmwareVersion { get { return YubiKeyDevice.FirmwareVersion; } }
        public TemplateStorageVersion? TemplateStorageVersion { get { return YubiKeyDevice.TemplateStorageVersion; } }
        public ImageProcessorVersion? ImageProcessorVersion { get { return YubiKeyDevice.ImageProcessorVersion; } }
        public int AutoEjectTimeout { get { return YubiKeyDevice.AutoEjectTimeout; } }
        public byte ChallengeResponseTimeout { get { return YubiKeyDevice.ChallengeResponseTimeout; } }
        public DeviceFlags DeviceFlags { get { return YubiKeyDevice.DeviceFlags; } }
        public bool ConfigurationLocked { get { return YubiKeyDevice.ConfigurationLocked; } }
        public Transport AvailableTransports { get { return YubiKeyDevice.AvailableTransports; } }

        public YubikeyInformation(YubiKeyDevice yubiKey)
        {
            this.YubiKeyDevice = yubiKey;
            this.PrettyName = PowershellYKText.FriendlyName(yubiKey);
        }

    }
}
