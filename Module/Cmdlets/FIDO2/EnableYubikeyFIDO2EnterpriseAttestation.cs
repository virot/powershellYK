/// <summary>
/// Enables enterprise attestation the YubiKey FIDO2 applet.
/// Enterprise attestation (EA) allows the YubiKey to provide detailed device information
/// during FIDO2 authentication, which can be useful for enterprise deployments.
/// Requires a YubiKey capable of Enterprise Attestation and administrator privileges on Windows.
/// Note: Enterprise attestation cannot be disabled without resetting the FIDO2 applet.
/// 
/// .EXAMPLE
/// Enable-YubiKeyFIDO2EnterpriseAttestation
/// Enables enterprise attestation on the connected YubiKey
/// 
/// .EXAMPLE
/// Enable-YubiKeyFIDO2EnterpriseAttestation -Confirm:$false
/// Enables enterprise attestation without confirmation prompt
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsLifecycle.Enable, "YubiKeyFIDO2EnterpriseAttestation", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class EnableYubikeyFIDO2CmdletEnterpriseAttestation : PSCmdlet
    {
        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to FIDO2 if not already authenticated
            if (YubiKeyModule._fido2PIN is null)
            {
                WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
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

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
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
