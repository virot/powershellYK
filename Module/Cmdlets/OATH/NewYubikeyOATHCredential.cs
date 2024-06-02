using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using VirotYubikey.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Piv;

namespace VirotYubikey.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.New, "YubikeyOATHCredential")]

    public class NewYubikeyOATH2CredentialCommand : PSCmdlet
    {
        

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "HOTP")]
        public SwitchParameter HOTP { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Type of OATH", ParameterSetName = "TOTP")]
        public SwitchParameter TOTP { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Issuer")]
        public string Issuer { get; set; } = "";

        [ValidateLength(1, 64)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Accountname")]
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
        }
        protected override void ProcessRecord()
        {
            // If already connected disconnect first
            if (YubiKeyModule._oathSession is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyOath");
                myPowersShellInstance.Invoke();
            }
            try
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


                YubiKeyModule._oathSession.AddCredential(newCredential);
            }
            catch (Exception e)
            {
                throw new Exception("Could not enumerate credentials", e);
            }
        }
    }
}