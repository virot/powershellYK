using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Switch, "YubiKeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SwitchYubikeyOTPCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
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
        protected override void ProcessRecord()
        {
            if (ShouldProcess("This will swap the two OTP slots of the YubiKey. Proceed?", "This will swap the two OTP slots of the YubiKey. Proceed?", "WARNING!"))
            {
                try
                {
                    using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                    {
                        otpSession.SwapSlots();
                        WriteInformation("YubiKey OTP slots swapped.", new string[] { "OTP", "info" });
                    }
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "OTPSwapError", ErrorCategory.OperationStopped, null));
                }
            }
        }
    }
}
