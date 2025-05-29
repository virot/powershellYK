/// <summary>
/// Retrieves information about the FIDO2 applet on a YubiKey.
/// Returns details about supported features, capabilities, and current settings.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2
/// Returns information about the FIDO2 applet on the connected YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2 | Format-List
/// Returns detailed FIDO2 information in a list format
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyFIDO2")]
    public class GetYubikeyFIDO2Cmdlet : PSCmdlet
    {
        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
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
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Get and output FIDO2 authenticator information
                AuthenticatorInfo info = fido2Session.AuthenticatorInfo;
                WriteObject(new Information(info));
            }
        }
    }
}
