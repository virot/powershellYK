using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.OTP;
using powershellYK.support.Validators;
using powershellYK.support.transform;
using Yubico.Core.Buffers;
using Yubico.Core.Devices.Hid;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Set, "YubikeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyOTPCommand : PSCmdlet
    {
        [TransformOTPSlot()]
        [ValidateOTPSlot()]
        [ArgumentCompletions("ShortPress","LongPress")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot")]
        public PSObject? Slot { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Yubico OTP")]
        public SwitchParameter YubicoOTP { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Generated Password")]
        public SwitchParameter StaticPassword { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows for Challenge-Response configuration with all defaults", ParameterSetName = "ChallengeResponse")]
        public SwitchParameter ChallengeResponse { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Public ID, defaults to the serialnumber", ParameterSetName = "Yubico OTP")]
        public byte[]? PublicID { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Private ID, defaults to random 6 bytes", ParameterSetName = "Yubico OTP")]
        public byte[]? PrivateID { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret key, defaults to random 16 bytes", ParameterSetName = "Yubico OTP")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret key, defaults to random 20 bytes", ParameterSetName = "ChallengeResponse")]
        public byte[]? SecretKey { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Upload to Yubicloud", ParameterSetName = "Yubico OTP")]
        public SwitchParameter Upload{ get; set; }
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

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Algorithm for challange response", ParameterSetName = "ChallengeResponse")]
        public ChallengeResponseAlgorithm Algorithm { get; set; } = ChallengeResponseAlgorithm.HmacSha1;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Require Touch", ParameterSetName = "ChallengeResponse")]
        public SwitchParameter RequireTouch { get; set; }


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
                if (Slot!.BaseObject is Slot)
                {
                    _slot = (Slot)Slot.BaseObject;
                }
                WriteDebug($"Working with {ParameterSetName}");
                if ((_slot == Yubico.YubiKey.Otp.Slot.ShortPress && !otpSession.IsShortPressConfigured) || (_slot == Yubico.YubiKey.Otp.Slot.LongPress && !otpSession.IsLongPressConfigured) || ShouldProcess($"Yubikey OTP {_slot}", "Set"))
                {
                    switch (ParameterSetName)
                    {
                        case "Yubico OTP":
                            Memory<byte> _publicID = new Memory<byte>(new byte[6]);
                            Memory<byte> _privateID = new Memory<byte>(new byte[6]);
                            Memory<byte> _secretKey = new Memory<byte>(new byte[16]);
                            ConfigureYubicoOtp configureyubicoOtp = otpSession.ConfigureYubicoOtp(_slot);
                            int? serial = YubiKeyModule._yubikey!.SerialNumber;
                            if (PublicID is null)
                            {
                                configureyubicoOtp = configureyubicoOtp.UseSerialNumberAsPublicId(_publicID);
                            }
                            else
                            {
                                _publicID = PublicID;
                                configureyubicoOtp = configureyubicoOtp.UsePublicId(PublicID);
                            }
                            if (PrivateID is null)
                            {
                                configureyubicoOtp = configureyubicoOtp.GeneratePrivateId(_privateID);
                            }
                            else
                            {
                                _privateID = PrivateID;
                                configureyubicoOtp = configureyubicoOtp.UsePublicId(PrivateID);
                            }
                            if (SecretKey is null)
                            {
                                configureyubicoOtp = configureyubicoOtp.GenerateKey(_secretKey);
                            }
                            else
                            {
                                _secretKey = SecretKey;
                                configureyubicoOtp = configureyubicoOtp.UseKey(SecretKey);
                            }
                            configureyubicoOtp.Execute();
                            if (PublicID is null || PrivateID is null || SecretKey is null)
                            {
                                YubicoOTP retur = new YubicoOTP(serial, _publicID.ToArray(), _privateID.ToArray(), _secretKey.ToArray(), "");
                                WriteObject(retur);
                            }
                            if (Upload.IsPresent)
                            {
                                // https://github.com/Yubico/yubikey-manager/blob/fbdae2bc12ba0451bcfc62372bc9191c10ecad0c/ykman/otp.py#L95
                                // TODO: Implement Upload to Yubicloud
                                WriteWarning("Upload to Yubicloud is not implemented yet");
                            }
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
                        case "ChallengeResponse":
                            Memory<byte> _CRsecretKey = new Memory<byte>(new byte[20]);
                            ConfigureChallengeResponse configureCR = otpSession.ConfigureChallengeResponse(_slot);
                            if (SecretKey is null)
                            { 
                                configureCR = configureCR.GenerateKey(_CRsecretKey);
                            }
                            else
                            {
                                _CRsecretKey = SecretKey;
                                configureCR = configureCR.UseKey(SecretKey);
                            }
                            if (RequireTouch.IsPresent)
                            {
                                configureCR = configureCR.UseButton();
                            }
                            if (Algorithm == ChallengeResponseAlgorithm.HmacSha1)
                            {
                                configureCR = configureCR.UseHmacSha1();
                            }
                            else if (Algorithm == ChallengeResponseAlgorithm.YubicoOtp)
                            {
                                configureCR = configureCR.UseYubiOtp();
                            }
                            configureCR.Execute();
                            if (SecretKey is null)
                            {
                                ChallangeResponse retur = new ChallangeResponse(_CRsecretKey.ToArray());
                                WriteObject(retur);
                            }
                            break;
                    }
                }
            }
        }

    }
}