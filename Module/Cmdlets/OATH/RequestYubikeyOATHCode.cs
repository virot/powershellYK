using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Oath;
using System.Collections.Generic;
using System;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsLifecycle.Request, "YubikeyOATHCode")]

    public class GetYubikeyOATHCodeCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Get codes for all credentials", ParameterSetName = "All")]
        public SwitchParameter All { get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential to generate code for", ParameterSetName = "Specific")]
        public Credential? Credential { get; set; }
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._oathSession is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyOath");
                myPowersShellInstance.Invoke();
            }
        }
        protected override void ProcessRecord()
        {
            if (ParameterSetName == "All")
            {
                IDictionary <Credential, Code> oathCodes = YubiKeyModule._oathSession!.CalculateAllCredentials();
                foreach (KeyValuePair<Credential, Code> oathCode in oathCodes)
                {
                    WriteObject(new powershellYK.OATH.Code.TOTP(oathCode.Key.Issuer != null ? oathCode.Key.Issuer : "", oathCode.Key.Name, oathCode.Value.ValidFrom, oathCode.Value.ValidUntil, oathCode.Value.Value != null ? oathCode.Value.Value : "")) ;
                }
            }
            else if (ParameterSetName == "Specific")
            {
                if (Credential is not null)
                {
                    Code oathCode = YubiKeyModule._oathSession!.CalculateCredential(Credential);

                    if ((oathCode.ValidFrom is not null) && (oathCode.ValidUntil is not null))
                    {
                        WriteObject(new powershellYK.OATH.Code.TOTP(Credential.Issuer != null ? Credential.Issuer : "", Credential.Name, oathCode.ValidFrom, oathCode.ValidUntil, oathCode.Value != null ? oathCode.Value : ""));
                    }
                    else
                    {
                        WriteObject(new powershellYK.OATH.Code.HOTP(Credential.Issuer != null ? Credential.Issuer : "", Credential.Name, oathCode.Value != null ? oathCode.Value : ""));
                    }
                }
            }
        }
    }
}