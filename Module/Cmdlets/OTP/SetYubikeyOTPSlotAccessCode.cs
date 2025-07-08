/// <summary>
/// Sets, changes or removes the OTP slot access code for a YubiKey.
/// The access code protects OTP slot configurations from unauthorized modifications.
/// Access codes are 6 bytes in length, provided as 12-character hex strings.
/// 
/// .EXAMPLE
/// # Set a new access code for a slot (when no access code exists)
/// Set-YubiKeySlotAccessCode -Slot LongPress -AccessCode "010203040506"
/// 
/// .EXAMPLE
/// # Change an existing slot access code
/// Set-YubiKeyOTPSlotAccessCode -Slot ShortPress -CurrentAccessCode "010203040506" -AccessCode "060504030201"
/// 
/// .EXAMPLE
/// # Remove slot access code protection (set to all zeros)
/// Set-YubiKeyOTPSlotAccessCode -Slot LongPress -CurrentAccessCode "010203040506" -RemoveAccessCode
/// 
/// .NOTES
/// Access codes must be provided as 12-character hex strings representing 6 bytes.
/// 
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Set, "YubiKeyOTPSlotAccessCode", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubiKeySlotAccessCodeCmdlet : PSCmdlet
    {
        // Specifies which YubiKey OTP slot to configure (ShortPress or LongPress)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot")]
        public Slot Slot { get; set; }

        // The new access code to set (will be converted to bytes, max 6 bytes)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New access code (12-character hex string)", ParameterSetName = "SetNewAccessCode")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New access code (12-character hex string)", ParameterSetName = "ChangeAccessCode")]
        [ValidateCount(12, 12)]
        public string? AccessCode { get; set; }

        // The current access code (required when changing or removing)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Current access code (12-character hex string)", ParameterSetName = "ChangeAccessCode")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Current access code (12-character hex string)", ParameterSetName = "RemoveAccessCode")]
        [ValidateCount(12, 12)]
        public string? CurrentAccessCode { get; set; }

        // Flag to remove access code protection (set to all zeros)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Remove access code protection", ParameterSetName = "RemoveAccessCode")]
        public SwitchParameter RemoveAccessCode { get; set; }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            // Verify that the YubiKey supports OTP
            var yk = (YubiKeyDevice)YubiKeyModule._yubikey!;
            bool hasOtp = false;
            if (yk.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.Otp))
            {
                hasOtp = true;
            }
            WriteDebug($"YubiKey OTP support: {hasOtp}");
            if (!hasOtp)
            {
                throw new Exception("The connected YubiKey does not support OTP functionality.");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                WriteDebug($"Working with parameter set: {ParameterSetName}");

                // Convert string access codes to byte arrays if provided
                byte[]? newAccessCodeBytes = null;
                byte[]? currentAccessCodeBytes = null;

                // Helper for string to byte[] conversion
                byte[] ConvertAccessCodeString(string code, string paramName)
                {
                    // Convert hex string to byte array using Hex helper class
                    return powershellYK.support.Hex.Decode(code);
                }

                if (AccessCode != null)
                {
                    newAccessCodeBytes = ConvertAccessCodeString(AccessCode, nameof(AccessCode));
                }
                else if (RemoveAccessCode.IsPresent)
                {
                    // Set to all zeros to remove access code protection
                    newAccessCodeBytes = new byte[SlotAccessCode.MaxAccessCodeLength];
                }

                if (CurrentAccessCode != null)
                {
                    currentAccessCodeBytes = ConvertAccessCodeString(CurrentAccessCode, nameof(CurrentAccessCode));
                }

                // Create SlotAccessCode objects
                SlotAccessCode? newAccessCode = null;
                SlotAccessCode? currentAccessCode = null;

                if (newAccessCodeBytes != null)
                {
                    newAccessCode = new SlotAccessCode(newAccessCodeBytes);
                }

                if (currentAccessCodeBytes != null)
                {
                    currentAccessCode = new SlotAccessCode(currentAccessCodeBytes);
                }

                // Confirm the operation if ShouldProcess is enabled
                if (!ShouldProcess("Update the slot access code?", "Continue?", "Confirm"))
                {
                    return;
                }

                try
                {
                    // Use UpdateSlot to change/remove access code without overwriting the key
                    var updateSlot = otpSession.UpdateSlot(Slot);

                    if (currentAccessCode != null)
                    {
                        updateSlot = updateSlot.UseCurrentAccessCode(currentAccessCode);
                    }

                    if (newAccessCode != null)
                    {
                        updateSlot = updateSlot.SetNewAccessCode(newAccessCode);
                    }

                    updateSlot.Execute();
                    WriteInformation("YubiKey slot access code operation completed.", new[] { "OTP", "Info" });
                }
                catch (Exception ex)
                {
                    // Show a meaningful message if the slot is already protected with a slot access code
                    if (ex.Message.Contains("YubiKey Operation Failed") && ex.Message.Contains("state of non-volatile memory is unchanged"))
                    {
                        WriteWarning("A slot access code is already set, call cmdlet again using -CurrentAccessCode.");
                    }
                    else
                    {
                        WriteError(new ErrorRecord(ex, "SetYubiKeySlotAccessCodeError", ErrorCategory.InvalidOperation, null));
                    }
                }
            }
        }
    }
}