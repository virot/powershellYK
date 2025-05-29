/// <summary>
/// Configures various settings on a YubiKey.
/// Supports managing USB and NFC capabilities, touch eject settings, and secure transport mode.
/// Requires a YubiKey 5 or later with unlocked configuration.
/// 
/// .EXAMPLE
/// Set-YubiKey -UsbCapabilities OTP,FIDO2
/// Replaces current USB capabilities with OTP and FIDO2
/// 
/// .EXAMPLE
/// Set-YubiKey -EnableUsbCapabilities PIV -DisableUsbCapabilities OTP
/// Enables PIV and disables OTP over USB
/// 
/// .EXAMPLE
/// Set-YubiKey -TouchEject $true
/// Enables touch eject functionality
/// 
/// .EXAMPLE
/// Set-YubiKey -AutoEjectTimeout 30
/// Sets auto-eject timeout to 30 seconds
/// 
/// .EXAMPLE
/// Set-YubiKey -SecureTransportMode
/// Enables restricted NFC / secure transport mode
/// </summary>

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
    [Cmdlet(VerbsCommon.Set, "YubiKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyCommand : PSCmdlet
    {
        // Parameters for USB capabilities
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Replace current USB capabilities with.", ParameterSetName = "Replace USB capabilities")]
        public YubiKeyCapabilities UsbCapabilities { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Enable capabilities over USB", ParameterSetName = "Update USB capabilities")]
        public YubiKeyCapabilities EnableUsbCapabilities { get; set; } = YubiKeyCapabilities.None;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Disable capabilities over USB", ParameterSetName = "Update USB capabilities")]
        public YubiKeyCapabilities DisableUsbCapabilities { get; set; } = YubiKeyCapabilities.None;

        // Parameters for NFC capabilities
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Replace current NFC capabilities with.", ParameterSetName = "Replace NFC capabilities")]
        public YubiKeyCapabilities? NFCCapabilities { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Enable capabilities over NFC", ParameterSetName = "Update NFC capabilities")]
        public YubiKeyCapabilities EnableNFCCapabilities { get; set; } = YubiKeyCapabilities.None;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Disable capabilities over NFC", ParameterSetName = "Update NFC capabilities")]
        public YubiKeyCapabilities DisableNFCCapabilities { get; set; } = YubiKeyCapabilities.None;

        // Parameters for touch eject settings
        [Parameter(Mandatory = true, HelpMessage = "Allows loading/unloading the smartcard by touching the YubiKey.", ParameterSetName = "Update Touch Eject flag")]
        public bool TouchEject;

        [Parameter(Mandatory = true, HelpMessage = "Automatically eject after the given time. Implies -TouchEject:$True. Value in seconds.", ParameterSetName = "Set automatically eject")]
        public UInt16 AutoEjectTimeout = 0;

        // Parameters for secure transport mode
        [Parameter(Mandatory = true, ParameterSetName = "Set Restricted NFC", HelpMessage = "Enable Restricted NFC / Secure Transport Mode")]
        public SwitchParameter SecureTransportMode { get; set; }

        // Initialize processing
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                try
                {
                    // Create a new PowerShell instance to run Connect-Yubikey
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Check if configuration is locked
            if (YubiKeyModule._yubikey!.ConfigurationLocked == false)
            {
                // Verify YubiKey version
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    switch (ParameterSetName)
                    {
                        case "Set automatically eject":
                            // Configure auto-eject timeout
                            if (AutoEjectTimeout > 0)
                            {
                                YubiKeyModule._yubikey!.SetAutoEjectTimeout(AutoEjectTimeout);
                                YubiKeyModule._yubikey!.SetDeviceFlags(YubiKeyModule._yubikey!.DeviceFlags | DeviceFlags.TouchEject);
                                WriteWarning("YubiKey needs to be reinserted.");
                            }
                            break;

                        case "Update Touch Eject flag":
                            // Configure touch eject functionality
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
                            // Replace all USB capabilities
                            if ((UsbCapabilities.HasFlag(YubiKeyCapabilities.Otp) || ShouldProcess("powershellYK management", "Disable")))
                            {
                                YubiKeyModule._yubikey!.SetEnabledUsbCapabilities((YubiKeyCapabilities)UsbCapabilities);
                                WriteWarning("YubiKey will reboot, diconnecting powershellYK!");
                            }
                            break;

                        case "Update USB capabilities":
                            // Update specific USB capabilities
                            if (EnableUsbCapabilities != YubiKeyCapabilities.None || DisableUsbCapabilities != YubiKeyCapabilities.None)
                            {
                                YubiKeyCapabilities requestedUSBCapabilities = YubiKeyModule._yubikey!.EnabledUsbCapabilities;
                                requestedUSBCapabilities |= EnableUsbCapabilities;
                                requestedUSBCapabilities &= ~DisableUsbCapabilities;
                                if ((requestedUSBCapabilities.HasFlag(YubiKeyCapabilities.Otp) || ShouldProcess("powershellYK management", "Disable")))
                                {
                                    YubiKeyModule._yubikey!.SetEnabledUsbCapabilities(requestedUSBCapabilities);
                                    WriteWarning("YubiKey will reboot, diconnecting powershellYK!");
                                }
                            }
                            break;

                        case "Replace NFC capabilities":
                            // Replace all NFC capabilities
                            YubiKeyModule._yubikey!.SetEnabledUsbCapabilities((YubiKeyCapabilities)UsbCapabilities);
                            WriteWarning("YubiKey will reboot, diconnecting powershellYK!");
                            break;

                        case "Update NFC capabilities":
                            // Update specific NFC capabilities
                            if (EnableNFCCapabilities != YubiKeyCapabilities.None || DisableNFCCapabilities != YubiKeyCapabilities.None)
                            {
                                YubiKeyCapabilities requestedNFCCapabilities = YubiKeyModule._yubikey!.EnabledNfcCapabilities;
                                requestedNFCCapabilities |= EnableNFCCapabilities;
                                requestedNFCCapabilities &= ~DisableNFCCapabilities;
                                YubiKeyModule._yubikey!.SetEnabledNfcCapabilities(requestedNFCCapabilities);
                                WriteWarning("YubiKey will reboot, diconnecting powershellYK!");
                            }
                            break;

                        case "Set Restricted NFC":
                            // Configure secure transport mode (Restricted NFC)
                            if (YubiKeyModule._yubikey!.HasFeature(YubiKeyFeature.ManagementNfcRestricted) == false)
                            {
                                throw new Exception("Restricting NFC is not supported in this YubiKey firmware version.");
                            }
                            else
                            {
                                try
                                {
                                    // Attempt to set restricted NFC
                                    YubiKeyModule._yubikey!.SetIsNfcRestricted(true);
                                    WriteInformation("YubiKey NFC temporarily disabled.", new string[] { "YubiKey", "Info" });
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
                throw new Exception("Configuration is locked, See Unlock-Yubikey!");
            }
        }
    }
}