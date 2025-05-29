/// <summary>
/// Resets a YubiKey PIV application to factory settings.
/// Deletes all PIV credentials and restores default configuration.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// Reset-YubiKeyPIV
/// Resets the PIV application after confirmation
/// 
/// .EXAMPLE
/// Reset-YubiKeyPIV -Force
/// Resets the PIV application without confirmation
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(bool))]
    public class ResetYubikeyPIVCommand : Cmdlet
    {
        // Parameter to force reset without confirmation
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Force reset of the PIV applet")]
        public SwitchParameter Force { get; set; } = false;

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord in Reset-YubikeyPIV");
            // Confirm reset operation with user
            if (Force || ShouldProcess("This will delete all PIV credentials, and restore factory settings. Proceed?", "This will delete all PIV credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Set up key collector for authentication
                    pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    // Reset the PIV application
                    pivSession.ResetApplication();
                }
                WriteInformation("YubiKey PIV applet successfully reset.", new string[] { "PIV", "Reset" });
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
