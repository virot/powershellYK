/// <summary>
/// Retrieves information about the OTP configuration on a YubiKey.
/// Returns the status of both short-press and long-press slots, including whether they are configured
/// and if they require touch for operation.
/// Requires a YubiKey with OTP support.
/// 
/// .EXAMPLE
/// Get-YubiKeyOTP
/// Returns the configuration status of both OTP slots
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using powershellYK.OTP;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Piv.Objects;
using Yubico.YubiKey.Sample.PivSampleCode;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyOTP")]
    public class GetYubikeyOTPCommand : Cmdlet
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
            // Open a session with the YubiKey OTP application
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Get information about the short-press slot
                WriteObject(new Info($"Slot {Slot.ShortPress.ToString("d")} ({Slot.ShortPress})",
                    otpSession.IsShortPressConfigured,
                    otpSession.ShortPressRequiresTouch));

                // Get information about the long-press slot
                WriteObject(new Info($"Slot {Slot.LongPress.ToString("d")} ({Slot.LongPress})",
                    otpSession.IsLongPressConfigured,
                    otpSession.LongPressRequiresTouch));
            }
        }
    }
}
