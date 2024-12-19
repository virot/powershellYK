using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsLifecycle.Enable, "YubikeyFIDO2EnterpriseAttestation", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]

    public class EnableYubikeyFIDO2CmdletEnterpriseAttestation : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            // If no FIDO2 PIN exists, we need to connect to the FIDO2 application
            if (YubiKeyModule._fido2PIN is null)
            {
                WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2").Invoke();
                if (YubiKeyModule._fido2PIN is null)
                {
                    throw new Exception("Connect-YubikeyFIDO2 failed to connect FIDO2 application.");
                }
            }


            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                fido2Session.AuthenticatorInfo.Options!.Any(v => v.Key.Contains(AuthenticatorOptions.ep));
                if (!(fido2Session.AuthenticatorInfo.Options!.Any(v => v.Key.Contains(AuthenticatorOptions.ep))) || fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.ep) == OptionValue.False)
                {
                    throw new Exception("Enterprise attestation not supported by this YubiKey.");
                }
                if (ShouldProcess("Enterprise attestion cannot be disabled without resetting the FIDO2 applet.", "Enterprise attestion cannot be disabled without resetting the FIDO2 applet.", "Disable not possible."))
                {
                    fido2Session.TryEnableEnterpriseAttestation();
                }
            }
        }
    }
}
