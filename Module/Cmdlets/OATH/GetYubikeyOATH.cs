/// <summary>
/// Gets information about the OATH applet on a YubiKey.
/// Returns whether the OATH applet is password protected.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// 
/// .EXAMPLE
/// Get-YubiKeyOATH
/// Gets information about the OATH applet on the connected YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyOATH | Select-Object -ExpandProperty PasswordProtected
/// Gets only the password protection status of the OATH applet
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
    [Cmdlet(VerbsCommon.Get, "YubiKeyOATH")]
    public class GetYubikeyOATH2Command : Cmdlet
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

                // Create and output OATH information object
                var customObject = new
                {
                    PasswordProtected = oathSession.IsPasswordProtected,
                };
                WriteObject(customObject);
            }
        }
    }
}
