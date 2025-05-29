/// <summary>
/// Gets all OATH apllet credentials stored on a YubiKey.
/// Returns detailed information about each credential including issuer, account name, and type.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// 
/// .EXAMPLE
/// Get-YubiKeyOATHAccount
/// Gets all OATH credentials stored on the connected YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyOATHAccount | Where-Object { $_.Issuer -eq "Example" }
/// Gets OATH credentials filtered by issuer
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
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyOATHAccount")]
    public class GetYubikeyOATH2AccountlCommand : Cmdlet
    {
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

                // Get and output all credentials
                IList<Credential> credentials = oathSession.GetCredentials();
                foreach (Credential credential in credentials)
                {
                    WriteObject(credential);
                }
            }
        }
    }
}
