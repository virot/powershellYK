using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using powershellYK.OTP;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using powershellYK.support.Validators;


namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyOTP")]
    public class RemoveYubikeyOTPCommand : Cmdlet
    {
        [ValidateOTPSlot()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot", ParameterSetName = "Remove")]
        public PSObject Slot { get; set; }

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
            WriteDebug($"Trying to remove from: {Slot.BaseObject.ToString()}, Slot is of type: {Slot.BaseObject.GetType()}");
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                if (Slot.BaseObject is Slot)
                {
                    otpSession.DeleteSlot((Slot)Slot.BaseObject);
                }
                else if ((Slot.BaseObject is int) || (Slot.BaseObject is byte))
                {
                    if ((int)Slot.BaseObject == 1)
                    {
                        otpSession.DeleteSlot(Yubico.YubiKey.Otp.Slot.ShortPress);
                    }
                    else if ((int)Slot.BaseObject == 1)
                    {
                        otpSession.DeleteSlot(Yubico.YubiKey.Otp.Slot.LongPress);
                    }

                }
            }
        }

    }
}