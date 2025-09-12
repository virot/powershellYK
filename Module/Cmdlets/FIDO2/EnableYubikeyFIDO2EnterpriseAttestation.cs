/// <summary>
/// Enables enterprise attestation the YubiKey FIDO2 applet.
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
    [Cmdlet(VerbsLifecycle.Enable, "YubiKeyFIDO2EnterpriseAttestation")]
    public class EnableYubikeyFIDO2CmdletEnterpriseAttestation : PSCmdlet
    {
        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to a YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
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
                    WriteWarning("Enterprise attestation is already enabled on this YubiKey.");
                    return;
                }
                
                // Check if PIN is required (only when alwaysUv is enabled but clientPin is not set)
                bool alwaysUv = fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.alwaysUv) == OptionValue.True;
                bool clientPin = fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.True;
                
                if (alwaysUv && !clientPin)
                {
                    throw new Exception("Enabling Enterprise Attestation requires a PIN to be set when alwaysUv is enabled.");
                }
                
                // Set up key collector only if PIN authentication is needed
                if (clientPin)
                {
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                }
                
                // Enable enterprise attestation if supported by the YubiKey
                fido2Session.TryEnableEnterpriseAttestation();
            }
        }
    }
}
