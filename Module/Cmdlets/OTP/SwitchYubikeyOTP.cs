using System.Management.Automation;
using powershellYK.OTP;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;



namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Switch, "YubikeyOTP")]
    public class SwitchYubikeyOTPCommand : Cmdlet
    {
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
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                otpSession.SwapSlots();
            }
        }

    }
}