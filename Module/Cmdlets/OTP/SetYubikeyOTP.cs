﻿/// <summary>
/// Configures OTP (One-Time Password) and/or fixed password on a YubiKey.
/// Supports multiple modes including Yubico OTP, Static Password, and Challenge-Response.
/// Can configure either the short-press (1) or long-press slot (2) on the YubiKey.
/// 
/// .EXAMPLE
/// # Configure Yubico OTP with default settings
/// Set-YubiKeyOTP -Slot ShortPress -YubicoOTP
/// 
/// .EXAMPLE
/// # Configure Yubico OTP with custom settings
/// $publicId = [byte[]]@(1,2,3,4,5,6)
/// $privateId = [byte[]]@(7,8,9,10,11,12)
/// $secretKey = [byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
/// Set-YubiKeyOTP -Slot LongPress -YubicoOTP -PublicID $publicId -PrivateID $privateId -SecretKey $secretKey
/// 
/// .EXAMPLE
/// # Configure static password
/// $securePassword = ConvertTo-SecureString "MySecretPassword" -AsPlainText -Force
/// Set-YubiKeyOTP -Slot ShortPress -StaticPassword -Password $securePassword -KeyboardLayout US -AppendCarriageReturn
/// 
/// .EXAMPLE
/// # Configure generated static password
/// Set-YubiKeyOTP -Slot LongPress -StaticGeneratedPassword -PasswordLength 32 -KeyboardLayout ModHex
/// 
/// .EXAMPLE
/// # Configure Challenge-Response with HMAC-SHA1
/// Set-YubiKeyOTP -Slot ShortPress -ChallengeResponse -Algorithm HmacSha1 -RequireTouch
/// 
/// .EXAMPLE
/// # Configure Challenge-Response with custom key
/// $secretKey = [byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20)
/// Set-YubiKeyOTP -Slot LongPress -ChallengeResponse -SecretKey $secretKey -Algorithm YubicoOtp
/// 
/// .EXAMPLE
/// # Configure HOTP with default settings
/// Set-YubiKeyOTP -Slot ShortPress -HOTP
/// 
/// .EXAMPLE
/// # Configure HOTP with custom secret key
/// $secretKey = [byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20)
/// Set-YubiKeyOTP -Slot LongPress -HOTP -SecretKey $secretKey
/// 
/// .EXAMPLE
/// # Configure HOTP with base32 encoded secret and carriage return
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -Base32Secret "QRFJ7DTIVASL3PNYXWFIQAQN5RKUJD4U" -AppendCarriageReturn
/// 
/// .EXAMPLE
/// # Configure HOTP with hex encoded secret and carriage return
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -HexSecret "0102030405060708090a0b0c0d0e0f1011121314" -AppendCarriageReturn
/// 
/// .EXAMPLE
/// # Configure HOTP with TAB before OTP code for easier form navigation
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -Base32Secret "QRFJ7DTIVASL3PNYXWFIQAQN5RKUJD4U" -SendTabFirst
/// 
/// .EXAMPLE
/// # Configure HOTP with 8 digits instead of 6
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -Use8Digits
/// 
/// .EXAMPLE
/// # Configure HOTP with 8 digits, TAB, and carriage return
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -Use8Digits -SendTabFirst -AppendCarriageReturn
/// 
/// .EXAMPLE
/// # Set a new access code for a slot (when no access code exists)
/// Set-YubiKeyOTP -Slot LongPress -HOTP -AccessCode "010203040506"
/// 
/// .EXAMPLE
/// # Change an existing slot access code
/// Set-YubiKeyOTP -Slot ShortPress -HOTP -CurrentAccessCode "010203040506" -AccessCode "060504030201"
/// 
/// .EXAMPLE
/// # Authenticate with an existing access code to update slot configuration
/// Set-YubiKeyOTP -Slot LongPress -HOTP -CurrentAccessCode "010203040506" -Base32Secret "QRFJ7DTIVASL3PNYXWFIQAQN5RKUJD4U"
/// </summary>

// Imports
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.OTP;
using powershellYK.support.validators;
using powershellYK.support.transform;
using powershellYK.support;
using Yubico.Core.Buffers;
using Yubico.Core.Devices.Hid;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;

namespace powershellYK.Cmdlets.OTP
{

    [Cmdlet(VerbsCommon.Set, "YubiKeyOTP", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyOTPCommand : PSCmdlet
    {
        // Specifies which YubiKey OTP slot to configure (ShortPress or LongPress)
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey OTP Slot")]
        public Slot Slot { get; set; }

        // Configures the slot for Yubico OTP mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Yubico OTP")]
        public SwitchParameter YubicoOTP { get; set; }

        // Configures the slot for Static Password mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Password")]
        public SwitchParameter StaticPassword { get; set; }

        // Configures the slot for Static Generated Password mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration with all defaults", ParameterSetName = "Static Generated Password")]
        public SwitchParameter StaticGeneratedPassword { get; set; }

        // Configures the slot for Challenge-Response mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows for Challenge-Response configuration with all defaults", ParameterSetName = "ChallengeResponse")]
        public SwitchParameter ChallengeResponse { get; set; }

        // Public ID for Yubico OTP mode. If not specified, uses YubiKey serial number
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Public ID, defaults to YubiKey serial number", ParameterSetName = "Yubico OTP")]
        public byte[]? PublicID { get; set; }

        // Private ID for Yubico OTP mode. If not specified, generates random 6 bytes
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Private ID, defaults to random 6 bytes", ParameterSetName = "Yubico OTP")]
        public byte[]? PrivateID { get; set; }

        // Secret key for OTP modes. For Yubico OTP: 16 bytes, for Challenge-Response: 20 bytes, for HOTP: 20 bytes
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret Key, defaults to random 16 bytes", ParameterSetName = "Yubico OTP")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret Key, defaults to random 20 bytes", ParameterSetName = "ChallengeResponse")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Sets the Secret Key, defaults to random 20 bytes", ParameterSetName = "HOTP")]
        public byte[]? SecretKey { get; set; }

        // Flag to upload Yubico OTP configuration to YubiCloud
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Upload to YubiCloud", ParameterSetName = "Yubico OTP")]
        public SwitchParameter Upload { get; set; }

        // Static password to be configured (1-38 characters)

        [ValidateYubikeyPassword(1, 38)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Static password that will be set", ParameterSetName = "Static Password")]
        public SecureString? Password { get; set; }

        // Length of the generated static password (1-38 characters)
        [ValidateRange(1, 38)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Static password that will be set", ParameterSetName = "Static Generated Password")]
        public int PasswordLength { get; set; }

        // Keyboard layout to use for static passwords
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Keyboard layout to be used", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Keyboard layout to be used", ParameterSetName = "Static Generated Password")]
        public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.ModHex;

        // Flag to append carriage return (Enter) after credential output
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Append carriage return (Enter)", ParameterSetName = "Static Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Append carriage return (Enter)", ParameterSetName = "Static Generated Password")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Append carriage return (Enter)", ParameterSetName = "HOTP")]
        public SwitchParameter AppendCarriageReturn { get; set; }

        // Sends a TAB character before the OTP passcode when using HOTP mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Send TAB before passcode to help navigate UI", ParameterSetName = "HOTP")]
        public SwitchParameter SendTabFirst { get; set; }

        // Configures the slot for HMAC-based One-Time Password (HOTP) mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Allows configuration of HOTP mode", ParameterSetName = "HOTP")]
        public SwitchParameter HOTP { get; set; }

        // Algorithm to use for Challenge-Response mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Algorithm for Challenge-Response", ParameterSetName = "ChallengeResponse")]
        public ChallengeResponseAlgorithm Algorithm { get; set; } = ChallengeResponseAlgorithm.HmacSha1;

        // Flag to require touch for Challenge-Response operations
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Require Touch", ParameterSetName = "ChallengeResponse")]
        public SwitchParameter RequireTouch { get; set; }

        // Base32 encoded secret key for HOTP mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Base32 encoded secret key for HOTP", ParameterSetName = "HOTP")]
        public string? Base32Secret { get; set; }

        // Hex encoded secret key for HOTP mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Hex encoded secret key for HOTP", ParameterSetName = "HOTP")]
        public string? HexSecret { get; set; }

        // Use 8 digits instead of 6 for HOTP mode
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Use 8 digits instead of 6 for HOTP", ParameterSetName = "HOTP")]
        public SwitchParameter Use8Digits { get; set; }

        // The new access code to set (will be converted to bytes, max 6 bytes)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New access code (12-character hex string)")]
        [ValidateCount(12, 12)]
        public string? AccessCode { get; set; }

        // The current access code (required when changing or authenticating)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current access code (12-character hex string)")]
        [ValidateCount(12, 12)]
        public string? CurrentAccessCode { get; set; }

        // Initializes the cmdlet by ensuring a YubiKey is connected
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


        // Main processing method that configures the YubiKey OTP settings based on the selected mode
        protected override void ProcessRecord()
        {
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                try
                {
                    WriteDebug($"Working with {ParameterSetName}");
                    if ((Slot == Yubico.YubiKey.Otp.Slot.ShortPress && !otpSession.IsShortPressConfigured) ||
                        (Slot == Yubico.YubiKey.Otp.Slot.LongPress && !otpSession.IsLongPressConfigured) ||
                        ShouldProcess($"Yubikey OTP {Slot}", "Set"))
                    {
                        switch (ParameterSetName)
                        {
                            case "Yubico OTP":
                                // Configure Yubico OTP mode
                                Memory<byte> _publicID = new Memory<byte>(new byte[6]);
                                Memory<byte> _privateID = new Memory<byte>(new byte[6]);
                                Memory<byte> _secretKey = new Memory<byte>(new byte[16]);
                                ConfigureYubicoOtp configureyubicoOtp = otpSession.ConfigureYubicoOtp(Slot);
                                int? serial = YubiKeyModule._yubikey!.SerialNumber;

                                // Handle Public ID configuration
                                if (PublicID is null)
                                {
                                    configureyubicoOtp = configureyubicoOtp.UseSerialNumberAsPublicId(_publicID);
                                }
                                else
                                {
                                    _publicID = PublicID;
                                    configureyubicoOtp = configureyubicoOtp.UsePublicId(PublicID);
                                }

                                // Handle Private ID configuration
                                if (PrivateID is null)
                                {
                                    configureyubicoOtp = configureyubicoOtp.GeneratePrivateId(_privateID);
                                }
                                else
                                {
                                    _privateID = PrivateID;
                                    configureyubicoOtp = configureyubicoOtp.UsePublicId(PrivateID);
                                }

                                // Handle Secret Key configuration
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

                                // Return configuration if any defaults were used
                                if (PublicID is null || PrivateID is null || SecretKey is null)
                                {
                                    YubicoOTP retur = new YubicoOTP(serial, _publicID.ToArray(), _privateID.ToArray(), _secretKey.ToArray(), "");
                                    WriteObject(retur);
                                }

                                // Handle YubiCloud upload
                                if (Upload.IsPresent)
                                {
                                    // https://github.com/Yubico/yubikey-manager/blob/fbdae2bc12ba0451bcfc62372bc9191c10ecad0c/ykman/otp.py#L95
                                    // TODO: Implement Upload to YubiCloud
                                    // @virot: upload is no longer supported. Need to output a CSV file for manual upload.
                                    WriteWarning("Upload to YubiCloud functionality has been deprecated by Yubico.");
                                }
                                break;

                            case "Static Password":
                                // Configure static password mode
                                ConfigureStaticPassword staticpassword = otpSession.ConfigureStaticPassword(Slot);
                                staticpassword = staticpassword.WithKeyboard(KeyboardLayout);
                                staticpassword = staticpassword.SetPassword((Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password!))!).AsMemory());
                                if (AppendCarriageReturn.IsPresent)
                                {
                                    staticpassword = staticpassword.AppendCarriageReturn();
                                }
                                staticpassword.Execute();
                                break;

                            case "Static Generated Password":
                                // Configure static generated password mode
                                ConfigureStaticPassword staticgenpassword = otpSession.ConfigureStaticPassword(Slot);
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
                                // Configure challenge-response mode
                                Memory<byte> _CRsecretKey = new Memory<byte>(new byte[20]);
                                ConfigureChallengeResponse configureCR = otpSession.ConfigureChallengeResponse(Slot);

                                // Handle Secret Key configuration
                                if (SecretKey is null)
                                {
                                    configureCR = configureCR.GenerateKey(_CRsecretKey);
                                }
                                else
                                {
                                    _CRsecretKey = SecretKey;
                                    configureCR = configureCR.UseKey(SecretKey);
                                }

                                // Configure touch requirement
                                if (RequireTouch.IsPresent)
                                {
                                    configureCR = configureCR.UseButton();
                                }

                                // Configure algorithm
                                if (Algorithm == ChallengeResponseAlgorithm.HmacSha1)
                                {
                                    configureCR = configureCR.UseHmacSha1();
                                }
                                else if (Algorithm == ChallengeResponseAlgorithm.YubicoOtp)
                                {
                                    configureCR = configureCR.UseYubiOtp();
                                }

                                configureCR.Execute();

                                // Return configuration if default key was used
                                if (SecretKey is null)
                                {
                                    ChallangeResponse retur = new ChallangeResponse(_CRsecretKey.ToArray());
                                    WriteObject(retur);
                                }
                                break;

                            case "HOTP":
                                // Configure HOTP mode
                                Memory<byte> _HOTPsecretKey = new Memory<byte>(new byte[20]);
                                ConfigureHotp configureHOTP = otpSession.ConfigureHotp(Slot);

                                // Handle Secret Key configuration using Base32
                                if (Base32Secret != null)
                                {
                                    _HOTPsecretKey = powershellYK.support.Base32.Decode(Base32Secret);
                                    configureHOTP = configureHOTP.UseKey(_HOTPsecretKey);
                                }
                                // Handle Secret Key configuration using Hex
                                else if (HexSecret != null)
                                {
                                    _HOTPsecretKey = powershellYK.support.Hex.Decode(HexSecret);
                                    configureHOTP = configureHOTP.UseKey(_HOTPsecretKey);
                                }
                                else if (SecretKey is null)
                                {
                                    configureHOTP = configureHOTP.GenerateKey(_HOTPsecretKey);
                                }
                                else
                                {
                                    _HOTPsecretKey = SecretKey;
                                    configureHOTP = configureHOTP.UseKey(SecretKey);
                                }

                                // Handle access code logic
                                byte[]? newAccessCodeBytes = null;
                                byte[]? currentAccessCodeBytes = null;
                                if (AccessCode != null)
                                {
                                    newAccessCodeBytes = powershellYK.support.Hex.Decode(AccessCode);
                                }
                                if (CurrentAccessCode != null)
                                {
                                    currentAccessCodeBytes = powershellYK.support.Hex.Decode(CurrentAccessCode);
                                }
                                SlotAccessCode? newAccessCode = null;
                                SlotAccessCode? currentAccessCode = null;
                                if (currentAccessCodeBytes != null)
                                {
                                    currentAccessCode = new SlotAccessCode(currentAccessCodeBytes);
                                    configureHOTP = configureHOTP.UseCurrentAccessCode(currentAccessCode);
                                    // If AccessCode is not provided, preserve the current code
                                    if (newAccessCodeBytes == null)
                                    {
                                        newAccessCode = currentAccessCode;
                                        configureHOTP = configureHOTP.SetNewAccessCode(newAccessCode);
                                    }
                                }
                                if (newAccessCodeBytes != null)
                                {
                                    newAccessCode = new SlotAccessCode(newAccessCodeBytes);
                                    configureHOTP = configureHOTP.SetNewAccessCode(newAccessCode);
                                }

                                // Configure TAB before OTP if requested
                                if (SendTabFirst.IsPresent)
                                {
                                    configureHOTP = configureHOTP.SendTabFirst();
                                }

                                // Configure carriage return if requested
                                if (AppendCarriageReturn.IsPresent)
                                {
                                    configureHOTP = configureHOTP.AppendCarriageReturn();
                                }

                                // Configure 8 digits if requested
                                if (Use8Digits.IsPresent)
                                {
                                    configureHOTP = configureHOTP.Use8Digits();
                                }

                                configureHOTP.Execute();

                                // Return both Hex and Base32 representations of the key
                                WriteObject(new
                                {
                                    HexSecret = powershellYK.support.Hex.Encode(_HOTPsecretKey.ToArray()),
                                    Base32Secret = powershellYK.support.Base32.Encode(_HOTPsecretKey.ToArray())
                                });
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Show a message to guide the user into providing or correcting a slot access code
                    if (ex.Message.Contains("YubiKey Operation Failed") && ex.Message.Contains("state of non-volatile memory is unchanged"))
                    {
                        WriteWarning("The requested slot is protected with a slot access code. Either no access code was provided, or the provided code was incorrect. Please call the cmdlet again using -CurrentAccessCode with the correct code.");
                    }
                    else
                    {
                        WriteError(new ErrorRecord(ex, "SetYubiKeyOTPError", ErrorCategory.InvalidOperation, null));
                    }
                }
            }
        }
    }
}
