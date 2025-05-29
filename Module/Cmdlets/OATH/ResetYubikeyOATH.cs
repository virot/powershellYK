/// <summary>
/// Resets the OATH applet on a YubiKey to factory settings.
/// Deletes all OATH credentials and removes password protection.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Requires a YubiKey with OTP support.
/// Note: This operation cannot be undone.
/// 
/// .EXAMPLE
/// Reset-YubiKeyOATH
/// Resets the OATH applet on the connected YubiKey
/// 
/// .EXAMPLE
/// Reset-YubiKeyOATH -Confirm:$false
/// Resets the OATH applet without confirmation prompt
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
    [Cmdlet(VerbsCommon.Reset, "YubiKeyOATH", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetYubikeyOATHCommand : Cmdlet
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
            if (ShouldProcess("This will delete all OATH credentials, and restore factory settings. Proceed?", "This will delete all OATH credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                try
                {
                    // Perform OATH reset
                    using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                    {
                        oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                        oathSession.ResetApplication();
                        WriteInformation("YubiKey OATH applet successfully reset.", new string[] { "OATH", "Reset" });
                    }
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "OATHResetError", ErrorCategory.OperationStopped, null));
                }
            }
        }
    }
}

