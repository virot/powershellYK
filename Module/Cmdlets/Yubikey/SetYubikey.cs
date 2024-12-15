using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.OTP;
using powershellYK.support.validators;
using powershellYK.support.transform;
using Yubico.Core.Buffers;
using Yubico.Core.Devices.Hid;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Set, "Yubikey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyCommand : PSCmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Replace current USB capabilities with.", ParameterSetName = "Replace USB capabilities")]
        public YubiKeyCapabilities UsbCapabilities { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Enabled capabilities to USB", ParameterSetName = "Update USB capabilities")]
        public YubiKeyCapabilities EnableUsbCapabilities { get; set; } = YubiKeyCapabilities.None;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Disable capabilities to USB", ParameterSetName = "Update USB capabilities")]
        public YubiKeyCapabilities DisableUsbCapabilities { get; set; } = YubiKeyCapabilities.None;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Replace current NFC capabilities with.", ParameterSetName = "Replace NFC capabilities")]
        public YubiKeyCapabilities? NFCCapabilities { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Enable capabilities to NFC", ParameterSetName = "Update NFC capabilities")]
        public YubiKeyCapabilities EnableNFCCapabilities { get; set; } = YubiKeyCapabilities.None;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Disable capabilities to NFC", ParameterSetName = "Update NFC capabilities")]
        public YubiKeyCapabilities DisableNFCCapabilities { get; set; } = YubiKeyCapabilities.None;
        [Parameter(Mandatory = true, HelpMessage = "Allows loading/unloading the smartcard by touching the yubikey.", ParameterSetName = "Update Touch Eject flag")]
        public bool TouchEject;
        [Parameter(Mandatory = true, HelpMessage = "Automatically eject after the given time. Implies -TouchEject:$True. Value in seconds.", ParameterSetName = "Set automatically eject")]
        public UInt16 AutoEjectTimeout = 0;

        [Parameter(Mandatory = true, ParameterSetName = "Set Restricted NFC", HelpMessage = "Enable Restricted NFC / Secure Transport Mode")]
        public SwitchParameter SecureTransportMode { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        protected override void ProcessRecord()
        {
            if (YubiKeyModule._yubikey!.ConfigurationLocked == false)
            {
                // Can be used for Yubikeys 5 and later.
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    switch (ParameterSetName)
                    {
                        case "Set automatically eject":
                            if (AutoEjectTimeout > 0)
                            {
                                YubiKeyModule._yubikey!.SetAutoEjectTimeout(AutoEjectTimeout);
                                YubiKeyModule._yubikey!.SetDeviceFlags(YubiKeyModule._yubikey!.DeviceFlags | DeviceFlags.TouchEject);
                                WriteWarning("Yubikey needs to be reinserted.");
                            }
                            break;
                        case "Update Touch Eject flag":
                            if (TouchEject)
                            {
                                YubiKeyModule._yubikey!.SetDeviceFlags(YubiKeyModule._yubikey!.DeviceFlags | DeviceFlags.TouchEject);
                            }
                            else
                            {
                                YubiKeyModule._yubikey!.SetDeviceFlags(YubiKeyModule._yubikey!.DeviceFlags & ~DeviceFlags.TouchEject);
                            }
                            break;
                        case "Replace USB capabilities":
                            if ((UsbCapabilities.HasFlag(YubiKeyCapabilities.Otp) || ShouldProcess("powershellYK management", "Disable")))
                            {
                                YubiKeyModule._yubikey!.SetEnabledUsbCapabilities((YubiKeyCapabilities)UsbCapabilities);
                                WriteWarning("Yubikey will reboot, diconnecting powershellYK.");
                            }
                            break;
                        case "Update USB capabilities":
                            if (EnableUsbCapabilities != YubiKeyCapabilities.None || DisableUsbCapabilities != YubiKeyCapabilities.None)
                            {
                                YubiKeyCapabilities requestedUSBCapabilities = YubiKeyModule._yubikey!.EnabledUsbCapabilities;
                                requestedUSBCapabilities |= EnableUsbCapabilities;
                                requestedUSBCapabilities &= ~DisableUsbCapabilities;
                                if ((requestedUSBCapabilities.HasFlag(YubiKeyCapabilities.Otp) || ShouldProcess("powershellYK management", "Disable")))
                                {
                                    YubiKeyModule._yubikey!.SetEnabledUsbCapabilities(requestedUSBCapabilities);
                                    WriteWarning("Yubikey will reboot, diconnecting powershellYK.");
                                }
                            }
                            break;
                        case "Replace NFC capabilities":
                            YubiKeyModule._yubikey!.SetEnabledUsbCapabilities((YubiKeyCapabilities)UsbCapabilities);
                            WriteWarning("Yubikey will reboot, diconnecting powershellYK.");
                            break;
                        case "Update NFC capabilities":
                            if (EnableNFCCapabilities != YubiKeyCapabilities.None || DisableNFCCapabilities != YubiKeyCapabilities.None)
                            {
                                YubiKeyCapabilities requestedNFCCapabilities = YubiKeyModule._yubikey!.EnabledNfcCapabilities;
                                requestedNFCCapabilities |= EnableNFCCapabilities;
                                requestedNFCCapabilities &= ~DisableNFCCapabilities;
                                YubiKeyModule._yubikey!.SetEnabledNfcCapabilities(requestedNFCCapabilities);
                                WriteWarning("Yubikey will reboot, diconnecting powershellYK.");
                            }
                            break;

                        case "Set Restricted NFC":
                            // Check if the switch is present
                            if (SecureTransportMode.IsPresent)
                            {
                                var yubiKey = YubiKeyDevice.FindByTransport(Transport.All).First();

                                try
                                {
                                    // Attempt to set restricted NFC
                                    yubiKey.SetIsNfcRestricted(true);

                                    Console.WriteLine("YubiKey NFC now disabled. NFC will be re-enabled automatically the next time the YubiKey is connected to USB power.");
                                }
                                catch (NotSupportedException)
                                {
                                    throw new Exception("Restricting NFC is not supported in this YubiKey firmware version.");
                                }
                                catch (Exception)
                                {
                                    throw new Exception("Failed to restrict NFC.");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (YubiKeyModule._yubikey!.FirmwareVersion.Major < 5)
                {
                    throw new Exception("This command is only implemented for YubiKey 5 and later.");
                }
            }
            else
            {
                throw new Exception("Configuration is locked, See Unlock-Yubikey");
            }
        }

    }
}