/// <summary>
/// Enables enterprise attestation on the YubiKey FIDO2 applet.
/// Enterprise attestation (EA) allows the YubiKey to provide detailed device information
/// during FIDO2 authentication, which can be useful for enterprise deployments.
/// Requires a YubiKey capable of Enterprise Attestation and administrator privileges on Windows.
/// Note: Enterprise attestation is only disabled when resetting the FIDO2 applet.
/// 
/// .EXAMPLE
/// Enable-YubiKeyFIDO2EnterpriseAttestation
/// Enables enterprise attestation on the connected YubiKey
/// 
/// .EXAMPLE
/// Enable-YubiKeyFIDO2EnterpriseAttestation -InformationAction Continue
/// Enables enterprise attestation and displays informational messages
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
    [Cmdlet(VerbsLifecycle.Enable, "YubiKeyFIDO2EnterpriseAttestation")]
    public class EnableYubikeyFIDO2CmdletEnterpriseAttestation : PSCmdlet
    {
        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to FIDO2 if not already authenticated
            if (YubiKeyModule._fido2PIN is null)
            {
                WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                if (YubiKeyModule._fido2PIN is null)
                {
                    throw new Exception("Connect-YubikeyFIDO2 failed to connect to the FIDO2 applet!");
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
            // Create a FIDO2 session with the YubiKey
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Check if enterprise attestation is supported
                if (!fido2Session.AuthenticatorInfo.Options!.Any(v => v.Key.Contains(AuthenticatorOptions.ep)))
                {
                    throw new Exception("Enterprise attestation not supported by this YubiKey.");
                }

                // Check if enterprise attestation is already enabled
                if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.ep) == OptionValue.True)
                {
                    WriteInformation("Enterprise attestation is already enabled on this YubiKey.", new string[] { "FIDO2", "Info" });
                    return;
                }

                // Set up key collector for PIN operations (required by SDK)
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Enable enterprise attestation if supported by the YubiKey
                fido2Session.TryEnableEnterpriseAttestation();
                WriteInformation("Enterprise attestation has been successfully enabled on this YubiKey.", new string[] { "FIDO2", "Info" });
            }
        }
    }
}
