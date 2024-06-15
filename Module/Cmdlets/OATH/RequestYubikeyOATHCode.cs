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
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Get codes for all accounts", ParameterSetName = "All")]
        public SwitchParameter All { get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Account to generate code for", ParameterSetName = "Specific")]
        [Alias("Credential")]
        public Credential? Account { get; set; }
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (ParameterSetName == "All")
                {
                    IDictionary<Credential, Code> oathCodes = oathSession.CalculateAllCredentials();
                    foreach (KeyValuePair<Credential, Code> oathCode in oathCodes)
                    {
                        WriteObject(new powershellYK.OATH.Code.TOTP(oathCode.Key.Issuer != null ? oathCode.Key.Issuer : "", oathCode.Key.Name, oathCode.Value.ValidFrom, oathCode.Value.ValidUntil, oathCode.Value.Value != null ? oathCode.Value.Value : ""));
                    }
                }
                else if (ParameterSetName == "Specific")
                {
                    if (Account is not null)
                    {
                        Code oathCode = oathSession.CalculateCredential(Account);

                        if ((oathCode.ValidFrom is not null) && (oathCode.ValidUntil is not null))
                        {
                            WriteObject(new powershellYK.OATH.Code.TOTP(Account.Issuer != null ? Account.Issuer : "", Account.Name, oathCode.ValidFrom, oathCode.ValidUntil, oathCode.Value != null ? oathCode.Value : ""));
                        }
                        else
                        {
                            WriteObject(new powershellYK.OATH.Code.HOTP(Account.Issuer != null ? Account.Issuer : "", Account.Name, oathCode.Value != null ? oathCode.Value : ""));
                        }
                    }
                }
            }
        }
    }
}