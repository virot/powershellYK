/// <summary>
/// Removes password protection from the OATH applet on a YubiKey.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Requires a YubiKey with OTP support.
/// Note: This operation cannot be undone.
/// 
/// .EXAMPLE
/// Unprotect-YubiKeyOATH
/// Removes password protection from the OATH applet
/// 
/// .EXAMPLE
/// Unprotect-YubiKeyOATH -Confirm:$false
/// Removes password protection without confirmation prompt
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
using powershellYK.support.validators;
using System.Security;
using System.Net.Security;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsSecurity.Unprotect, "YubiKeyOATH", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class UnprotectYubikeyOATH2Cmdlet : PSCmdlet
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
            if (ShouldProcess("This will remove the password for the OATH application. Proceed?", "This will remove the password for the OATH application. Proceed?", "WARNING!"))
            {
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    // Remove password protection if enabled
                    if (oathSession.IsPasswordProtected)
                    {
                        oathSession.UnsetPassword();
                        YubiKeyModule._OATHPassword = null;
                        WriteInformation("YubiKey OATH applet password removed.", new string[] { "OATH", "Info" });
                    }
                }
            }
        }
    }
}
