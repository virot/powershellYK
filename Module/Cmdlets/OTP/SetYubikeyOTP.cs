﻿using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.OTP;
using powershellYK.support.Validators;
using Yubico.Core.Devices.Hid;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Set, "YubikeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyOTPCommand : PSCmdlet
    {
        [ValidateOTPSlot()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot")]
        public PSObject? Slot { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Yubico OTP")]
        public SwitchParameter YubicoOTP { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Generated Password")]
        public SwitchParameter StaticPassword { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Public ID, defaults to the serialnumber", ParameterSetName = "Yubico OTP")]
        public byte[]? PublicID { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Private ID, defaults to random 6 bytes", ParameterSetName = "Yubico OTP")]
        public byte[]? PrivateID { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret key, defaults to random 16 bytes ", ParameterSetName = "Yubico OTP")]
        public byte[]? SecretKey { get; set; }
        [ValidateYubikeyPassword(1,38)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Static password that will be set", ParameterSetName = "Static Password")]
        public SecureString? Password { get; set; }
        [ValidateRange(1, 38)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Static password that will be set", ParameterSetName = "Static Generated Password")]
        public int PasswordLength { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Keyboard to be used", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Keyboard to be used", ParameterSetName = "Static Generated Password")]
        public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.ModHex;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Append carriage return ", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Append carriage return ", ParameterSetName = "Static Generated Password")]
        public SwitchParameter AppendCarriageReturn { get; set; }

        private Slot _slot { get; set; }



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
                // Set an internal Slot variable to work with.
                if (Slot!.BaseObject is Slot)
                {
                    _slot = (Slot)Slot.BaseObject;
                }
                else if ((int)Slot.BaseObject == 1)
                {
                    _slot = Yubico.YubiKey.Otp.Slot.ShortPress;
                }
                else if ((int)Slot.BaseObject == 1)
                {
                    _slot = Yubico.YubiKey.Otp.Slot.LongPress;
                }
                WriteDebug($"Working with {ParameterSetName}");
                if ((_slot == Yubico.YubiKey.Otp.Slot.ShortPress && !otpSession.IsShortPressConfigured) || (_slot == Yubico.YubiKey.Otp.Slot.LongPress && !otpSession.IsLongPressConfigured) || ShouldProcess($"Yubikey OTP {_slot}", "Set"))
                {
                    switch (ParameterSetName)
                    {
                        case "dasdas":
                            break;

                        case "Static Password":
                            ConfigureStaticPassword staticpassword = otpSession.ConfigureStaticPassword(_slot);
                            staticpassword = staticpassword.WithKeyboard(KeyboardLayout);
                            staticpassword = staticpassword.SetPassword((Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password!))!).AsMemory());
                            if (AppendCarriageReturn.IsPresent)
                            {
                                staticpassword = staticpassword.AppendCarriageReturn();
                            }
                            staticpassword.Execute();
                            break;
                        case "Static Generated Password":
                            ConfigureStaticPassword staticgenpassword = otpSession.ConfigureStaticPassword(_slot);
                            Memory<char> generatedPassword = new Memory<char>(new char[PasswordLength]);
                            staticgenpassword = staticgenpassword.WithKeyboard(KeyboardLayout);
                            staticgenpassword = staticgenpassword.GeneratePassword(generatedPassword);
                            if (AppendCarriageReturn.IsPresent)
                            {
                                staticgenpassword = staticgenpassword.AppendCarriageReturn();
                            }
                            staticgenpassword.Execute();
                            break;
                    }
                }
            }
        }

    }
}