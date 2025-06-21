/// <summary>
/// Sets, changes or removes the OTP slot access code for a YubiKey.
/// The access code protects OTP slot configurations from unauthorized modifications.
/// Access codes are 6 bytes in length.
/// 
/// .EXAMPLE
/// # Set a new access code for a slot (when no access code exists)
/// Set-YubiKeySlotAccessCode -Slot LongPress -AccessCode "123456"
/// 
/// .EXAMPLE
/// # Change an existing access code
/// Set-YubiKeySlotAccessCode -Slot ShortPress -CurrentAccessCode "123456" -AccessCode "654321"
/// 
/// .EXAMPLE
/// # Remove access code protection (set to all zeros)
/// Set-YubiKeySlotAccessCode -Slot LongPress -CurrentAccessCode "123456" -RemoveAccessCode
/// 
/// .EXAMPLE
/// # Set access code using byte array (6 bytes)
/// $accessCodeBytes = [byte[]]@(1,2,3,4,5,6)
/// Set-YubiKeySlotAccessCode -Slot ShortPress -AccessCodeBytes $accessCodeBytes
/// 
/// .NOTES
/// Setting or changing the access code will overwrite the selected slot's configuration.
/// This operation cannot be undone and will erase any existing secret or configuration in the slot.
/// 
/// .LINK
/// https://docs.yubico.com/yesdk/users-manual/application-otp/how-to-slot-access-codes.html
/// 
/// </summary>

// Imports
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.OTP;
using powershellYK.support.validators;
using powershellYK.support.transform;
using powershellYK.support;
using Yubico.Core.Buffers;
using Yubico.Core.Devices.Hid;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Set, "YubiKeySlotAccessCode", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubiKeySlotAccessCodeCmdlet : PSCmdlet
    {
        // Specifies which YubiKey OTP slot to configure (ShortPress or LongPress)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot")]
        public Slot Slot { get; set; }

        // The new access code to set (will be converted to bytes, max 6 bytes)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New access code (max 6 bytes as string)", ParameterSetName = "SetNewAccessCode")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New access code (max 6 bytes as string)", ParameterSetName = "ChangeAccessCode")]
        public string? AccessCode { get; set; }

        // The current access code (required when changing or removing)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Current access code (max 6 bytes as string)", ParameterSetName = "ChangeAccessCode")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Current access code (max 6 bytes as string)", ParameterSetName = "RemoveAccessCode")]
        public string? CurrentAccessCode { get; set; }

        // Flag to remove access code protection (set to all zeros)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Remove access code protection", ParameterSetName = "RemoveAccessCode")]
        public SwitchParameter RemoveAccessCode { get; set; }

        // Access code as byte array (6 bytes)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Access code as byte array (6 bytes)", ParameterSetName = "SetNewAccessCode")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Access code as byte array (6 bytes)", ParameterSetName = "ChangeAccessCode")]
        [ValidateCount(6, 6)]
        public byte[]? AccessCodeBytes { get; set; }

        // Current access code as byte array (6 bytes)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current access code as byte array (6 bytes)", ParameterSetName = "ChangeAccessCode")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current access code as byte array (6 bytes)", ParameterSetName = "RemoveAccessCode")]
        [ValidateCount(6, 6)]
        public byte[]? CurrentAccessCodeBytes { get; set; }

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
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                WriteDebug($"Working with parameter set: {ParameterSetName}");

                // Convert string access codes to byte arrays if provided
                byte[]? newAccessCodeBytes = null;
                byte[]? currentAccessCodeBytes = null;

                // Helper for string to byte[] conversion with validation
                byte[] ConvertAccessCodeString(string code, string paramName)
                {
                    var bytes = System.Text.Encoding.ASCII.GetBytes(code);
                    if (bytes.Length != SlotAccessCode.MaxAccessCodeLength)
                    {
                        ThrowTerminatingError(new ErrorRecord(
                            new ArgumentException($"{paramName} must be exactly {SlotAccessCode.MaxAccessCodeLength} bytes when encoded as ASCII."),
                            "AccessCodeInvalidLength",
                            ErrorCategory.InvalidArgument,
                            code));
                    }
                    return bytes;
                }

                if (AccessCode != null)
                {
                    newAccessCodeBytes = ConvertAccessCodeString(AccessCode, nameof(AccessCode));
                }
                else if (AccessCodeBytes != null)
                {
                    newAccessCodeBytes = AccessCodeBytes;
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
                else if (CurrentAccessCodeBytes != null)
                {
                    currentAccessCodeBytes = CurrentAccessCodeBytes;
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
                if (!ShouldProcess("This will overwrite the current slot configuration including secrets!", "Continue?", "Confirm"))
                {
                    return;
                }

                try
                {
                    // Create a basic HOTP configuration with access code support
                    var configureHOTP = otpSession.ConfigureHotp(Slot);
                    var hmacKey = new byte[20];
                    configureHOTP.UseKey(hmacKey);


                    // Apply access code changes
                    if (currentAccessCode != null)
                    {
                        configureHOTP.UseCurrentAccessCode(currentAccessCode);
                    }

                    if (newAccessCode != null)
                    {
                        configureHOTP.SetNewAccessCode(newAccessCode);
                    }

                    configureHOTP.Execute();
                    WriteInformation("YubiKey slot access code operation completed.", new[] { "OTP", "Info" });
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "SetYubiKeySlotAccessCodeError", ErrorCategory.InvalidOperation, null));
                }
            }
        }
    }
} 