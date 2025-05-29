/// <summary>
/// Requests a challenge-response calculation from a YubiKey OTP slot.
/// Supports both HMAC-SHA1 and Yubico OTP algorithms.
/// Requires a YubiKey with OTP support and a configured challenge-response slot.
/// 
/// .EXAMPLE
/// Request-YubiKeyOTPChallange -Slot ShortPress -Phrase "Hello World"
/// Calculates HMAC-SHA1 response for the given challenge
/// 
/// .EXAMPLE
/// Request-YubiKeyOTPChallange -Slot LongPress -Phrase "Test123" -YubikeyOTP $true
/// Calculates Yubico OTP response for the given challenge
/// </summary>

// Imports
using System.Management.Automation;
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
        // Parameters for challenge configuration
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "YubiOTP Slot")]
        public Slot Slot { get; set; }

        [TransformHexInput()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Phrase")]
        public PSObject? Phrase { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Use YubiOTP over HMAC-SHA1")]
        public Boolean YubikeyOTP { get; set; } = false;

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    // Create a new PowerShell instance to run Connect-Yubikey
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

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Open a session with the YubiKey OTP application
            using (var otpSession = new OtpSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Calculate challenge-response
                CalculateChallengeResponse challange = otpSession.CalculateChallengeResponse(Slot);
                challange = challange.UseChallenge((byte[])Phrase!.BaseObject);
                challange.UseYubiOtp(YubikeyOTP);

                // Return the response as a string
                WriteObject(Converter.ByteArrayToString(challange.GetDataBytes().ToArray()));
            }
        }
    }
}
