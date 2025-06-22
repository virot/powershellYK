/// <summary>
/// Swaps the configuration of the two OTP slots on a YubiKey.
/// This operation exchanges the settings between the short-press and long-press slots.
/// Requires a YubiKey with OTP support.
/// 
/// .EXAMPLE
/// Switch-YubiKeyOTP
/// Swaps the configuration between the short-press and long-press OTP slots
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Switch, "YubiKeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SwitchYubikeyOTPCommand : Cmdlet
    {
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
            if (ShouldProcess("This will swap the two OTP slots of the YubiKey. Proceed?", "This will swap the two OTP slots of the YubiKey. Proceed?", "WARNING!"))
            {
                try
                {
                    // Open a session with the YubiKey OTP application
                    using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                    {
                        // Swap the OTP slot configurations
                        otpSession.SwapSlots();
                        WriteInformation("YubiKey OTP slots swapped.", new string[] { "OTP", "info" });
                    }
                }
                catch (Exception ex)
                {
                    // If either slot is protected with an access code show a meaningful error
                    if (ex.Message.Contains("Warning, state of non-volatile memory is unchanged."))
                    {
                        WriteError(new ErrorRecord(new Exception("Either one or both slots are protected with a slot access code."), "OTPSwapAccessCodeError", ErrorCategory.SecurityError, null));
                    }
                    else
                    {
                        WriteError(new ErrorRecord(ex, "OTPSwapError", ErrorCategory.OperationStopped, null));
                    }
                }
            }
        }
    }
}
