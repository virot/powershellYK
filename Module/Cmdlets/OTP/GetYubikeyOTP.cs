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
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                WriteObject(new Info(Slot.ShortPress.ToString(), otpSession.IsShortPressConfigured, otpSession.ShortPressRequiresTouch));
                WriteObject(new Info(Slot.LongPress.ToString(), otpSession.IsLongPressConfigured, otpSession.LongPressRequiresTouch));
            }
        }

    }
}
