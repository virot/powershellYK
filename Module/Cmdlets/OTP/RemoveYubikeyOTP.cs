/// <summary>
/// Removes the OTP configuration from a specified slot on a YubiKey.
/// This operation deletes all settings and data from the selected slot.
/// Requires a YubiKey with OTP support.
/// 
/// .EXAMPLE
/// Remove-YubiKeyOTP -Slot ShortPress
/// Removes the OTP configuration from the short-press slot
/// 
/// .EXAMPLE
/// Remove-YubiKeyOTP -Slot LongPress -CurrentAccessCode "010203040506"
/// Removes the OTP configuration from the long-press slot when a slot access code is set
/// 
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using powershellYK.support.validators;
using powershellYK.support.transform;
using Yubico.YubiKey.Oath;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Remove, "YubiKeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyOTPCommand : Cmdlet
    {
        // Parameters for slot selection
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "YubiOTP Slot", ParameterSetName = "Remove")]
        public Slot Slot { get; set; }

        // The current access code (12-character hex string) if the slot is protected
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current access code (12-character hex string)", ParameterSetName = "Remove")]
        [ValidateCount(12, 12)]
        public string? CurrentAccessCode { get; set; }

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    // Create a new PowerShell instance to run Connect-Yubikey
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
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
            // Confirm operation with user
            if (ShouldProcess($"This will remove the OTP configuration in slot {Slot.ToString("d")} ({Slot}). Proceed?", $"This will remove the OTP configuration in slot {Slot.ToString("d")} ({Slot}). Proceed?", "Warning"))
            {
                // Open a session with the YubiKey OTP application
                using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Check if the slot is configured
                    if ((Slot == Slot.ShortPress && !otpSession.IsShortPressConfigured) ||
                        (Slot == Slot.LongPress && !otpSession.IsLongPressConfigured))
                    {
                        WriteWarning($"Slot {Slot.ToString("d")} ({Slot}) is not configured.");
                        return;
                    }

                    // Delete the slot configuration
                    var deleteSlot = otpSession.DeleteSlotConfiguration(Slot);
                    // If CurrentAccessCode is provided, use it
                    if (CurrentAccessCode != null)
                    {
                        // Convert hex string to byte array using Hex helper class
                        var currentAccessCodeBytes = powershellYK.support.Hex.Decode(CurrentAccessCode);
                        var slotAccessCode = new SlotAccessCode(currentAccessCodeBytes);
                        deleteSlot = deleteSlot.UseCurrentAccessCode(slotAccessCode);
                    }
                    try
                    {
                        deleteSlot.Execute(); // Note: Deletion may return an error even when successful
                        WriteInformation($"Removed OTP configuration from slot {Slot.ToString("d")}", new string[] { "OTP", "Info" });
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("YubiKey Operation Failed") && ex.Message.Contains("state of non-volatile memory is unchanged"))
                        {
                            WriteWarning("The requested slot is protected with a slot access code. Either no access code was provided, or the provided code was incorrect. Please call the cmdlet again using -CurrentAccessCode with the correct code.");
                        }
                        else
                        {
                            WriteError(new ErrorRecord(ex, "RemoveYubiKeyOTPError", ErrorCategory.InvalidOperation, null));
                        }
                    }
                }
            }
        }
    }
}