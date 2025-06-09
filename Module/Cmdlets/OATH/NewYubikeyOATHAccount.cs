/// <summary>
/// Creates a new OATH credential on a YubiKey.
/// Supports both TOTP and HOTP credential types.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Note: credential is created in the OATH applet requiring a companion app.
/// 
/// .EXAMPLE
/// New-YubiKeyOATHAccount -TOTP -Issuer "Example" -Accountname "user@example.com" -Secret "JBSWY3DPEHPK3PXP" -Period Thirty
/// Creates a new TOTP credential with 30-second period
/// 
/// .EXAMPLE
/// New-YubiKeyOATHAccount -HOTP -Issuer "Example" -Accountname "user@example.com" -Secret "JBSWY3DPEHPK3PXP" -Counter
/// Creates a new HOTP credential with counter
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.New, "YubiKeyOATHAccount", DefaultParameterSetName = "TOTP")]
    public class NewYubikeyOATH2AccountCommand : PSCmdlet
    {
        // Parameters for OATH type selection
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "HOTP")]
        public SwitchParameter HOTP { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "TOTP")]
        public SwitchParameter TOTP { get; set; }

        // Parameters for credential information
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Issuer")]
        public string Issuer { get; set; } = "";

        [ValidateLength(1, 64)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Account name")]
        public string Accountname { get; set; } = "";

        [ValidateSet("SHA1", "SHA256", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Algorithm")]
        public HashAlgorithm Algorithm { get; set; } = HashAlgorithm.Sha1;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Secret")]
        public string Secret { get; set; } = "";

        // Parameters for TOTP configuration
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Period for credential", ParameterSetName = "TOTP")]
        public CredentialPeriod Period { get; set; }
        //[Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "RequireTouch")]
        //public SwitchParameter RequireTouch { get; set; }

        // Parameters for HOTP configuration
        [ValidateRange(6, 8)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Digits")]
        public int Digits { get; set; } = 6;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Counter", ParameterSetName = "HOTP")]
        public SwitchParameter Counter { get; set; }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Create credential object with specified parameters
            Credential newCredential = new Credential
            {
                Issuer = Issuer,
                AccountName = Accountname,
                Secret = Secret,
                Period = Period,
                Algorithm = Algorithm,
                Type = HOTP.IsPresent ? CredentialType.Hotp : CredentialType.Totp,
                //RequireTouch = RequireTouch.IsPresent,
                Digits = Digits,
                Counter = Counter.IsPresent ? 0 : null
            };

            // Add credential to YubiKey
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                oathSession.AddCredential(newCredential);
            }
        }
    }
}
