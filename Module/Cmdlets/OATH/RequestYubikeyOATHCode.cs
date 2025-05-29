/// <summary>
/// Requests OATH codes from the YubiKey OATH applet.
/// Can generate codes for all credentials or a specific credential.
/// Requires a YubiKey with OTP support.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// 
/// .EXAMPLE
/// Request-YubiKeyOATHCode -All
/// Gets codes for all OATH credentials
/// 
/// .EXAMPLE
/// $cred = Get-YubiKeyOATHAccount | Select-Object -First 1
/// Request-YubiKeyOATHCode -Account $cred
/// Gets code for a specific OATH credential
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
using System.Collections.Generic;
using System;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsLifecycle.Request, "YubiKeyOATHCode", DefaultParameterSetName = "All")]
    public class GetYubikeyOATHCodeCommand : PSCmdlet
    {
        // Parameters for code generation
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Get codes for all accounts", ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Account to generate code for", ParameterSetName = "Specific")]
        [Alias("Credential")]
        public Credential? Account { get; set; }

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
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Generate codes for all credentials
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
