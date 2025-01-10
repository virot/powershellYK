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
        //[ValidateOTPSlot()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "YubiOTP Slot", ParameterSetName = "Remove")]
        public Slot Slot { get; set; }

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
            if (ShouldProcess($"This will remove the OTP configuration in slot {Slot.ToString("d")} ({Slot}). Proceed?", $"This will remove the OTP configuration in slot {Slot.ToString("d")} ({Slot}). Proceed?", "Warning"))
            {
                using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Check if the slot is configured, if not, Write Warning and continue
                    if ((Slot == Slot.ShortPress && !otpSession.IsShortPressConfigured) || (Slot == Slot.LongPress && !otpSession.IsLongPressConfigured))
                    {
                        WriteWarning($"Slot {Slot.ToString("d")} ({Slot}) is not configured.");
                        return;
                    }
                    var deleteSlot = otpSession.DeleteSlotConfiguration(Slot);
                    deleteSlot.Execute();
                }
            }
        }

    }
}