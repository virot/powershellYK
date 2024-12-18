﻿using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using powershellYK.support.validators;
using powershellYK.support.transform;
using Yubico.YubiKey.Oath;


namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyOTPCommand : Cmdlet
    {
        [TransformOTPSlot()]
        [ValidateOTPSlot()]
        [ArgumentCompletions("ShortPress", "LongPress")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "YubiOTP Slot", ParameterSetName = "Remove")]
        public PSObject? Slot { get; set; }
        private Slot _slot { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
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
            // Set an internal Slot variable to work with.
            if (Slot!.BaseObject is Slot)
            {
                _slot = (Slot)Slot.BaseObject;
            }
            if (ShouldProcess($"Yubikey OTP {_slot}", "Set"))
            {
                using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    otpSession.DeleteSlot(_slot);
                }
            }
        }

    }
}