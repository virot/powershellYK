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
    [Cmdlet(VerbsCommon.New, "YubikeyOATHAccount", DefaultParameterSetName = "TOTP")]

    public class NewYubikeyOATH2AccountCommand : PSCmdlet
    {


        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "HOTP")]
        public SwitchParameter HOTP { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "TOTP")]
        public SwitchParameter TOTP { get; set; }

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

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Period for credential", ParameterSetName = "TOTP")]
        public CredentialPeriod Period { get; set; }
        //[Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "RequireTouch")]
        //public SwitchParameter RequireTouch { get; set; }

        [ValidateRange(6, 8)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Digits")]
        public int Digits { get; set; } = 6;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Counter", ParameterSetName = "HOTP")]
        public SwitchParameter Counter { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        protected override void ProcessRecord()
        {
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

            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                oathSession.AddCredential(newCredential);
            }
        }
    }
}
