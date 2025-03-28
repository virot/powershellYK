﻿using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using powershellYK.OTP;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using Yubico.YubiKey;
using Yubico.YubiKey.Otp;
using Yubico.YubiKey.Otp.Operations;
using Yubico.YubiKey.Piv.Objects;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsLifecycle.Request, "YubiKeyOTPChallange")]
    public class RequestYubikeyOTPChallangeCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "YubiOTP Slot")]
        public Slot Slot;
        [TransformHexInput()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Phrase")]
        public PSObject? Phrase;
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Use YubiOTP over HMAC-SHA1")]
        public Boolean YubikeyOTP = false;


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
                CalculateChallengeResponse challange = otpSession.CalculateChallengeResponse(Slot);
                challange = challange.UseChallenge((byte[])Phrase!.BaseObject);
                challange.UseYubiOtp(YubikeyOTP);
                WriteObject(Converter.ByteArrayToString(challange.GetDataBytes().ToArray()));
            }
        }

    }
}
