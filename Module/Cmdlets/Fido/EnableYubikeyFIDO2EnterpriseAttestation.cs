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
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
            }
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                fido2Session.AuthenticatorInfo.Options!.Any(v => v.Key.Contains(AuthenticatorOptions.ep));
                if (! (fido2Session.AuthenticatorInfo.Options!.Any(v => v.Key.Contains(AuthenticatorOptions.ep))) || fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.ep) == OptionValue.False)
                {
                    throw new Exception("Enterprise attestation not supported by this YubiKey.");
                }
                if (ShouldProcess("Enterprise attestion cannot be disabled without resetting fido2 application", "Enterprise attestion cannot be disabled without resetting fido2 application", "Disable not possible"))
                {
                    fido2Session.TryEnableEnterpriseAttestation();
                }
            }
        }
    }
}