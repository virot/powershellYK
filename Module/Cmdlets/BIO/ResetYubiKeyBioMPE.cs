/// <summary>
/// Resets a YubiKey Bio MPE to factory settings.
/// Deletes all stored data and restores default configuration.
/// Requires confirmation before proceeding.
/// 
/// .EXAMPLE
/// Reset-YubiKeyBioMPE
/// Resets the connected YubiKey Bio MPE to factory settings after confirmation
/// 
/// .EXAMPLE
/// Reset-YubiKeyBioMPE -Confirm:$false
/// Resets the connected YubiKey Bio MPE without confirmation prompt
/// </summary>

// Imports
using System.Management.Automation;

namespace powershellYK.Cmdlets.Bio
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyBioMPE", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetYubiKeyCmdlet : PSCmdlet
    {
        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
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
            // Confirm reset operation with user
            if (!ShouldProcess("This will delete all stored data and restore factory settings. Proceed?", "This will delete all stored data and restore factory settings. Proceed?", "WARNING!"))
            {
                return;
            }

            // Verify YubiKey is connected
            if (YubiKeyModule._yubikey is null)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new InvalidOperationException("No YubiKey is connected."),
                        "NoYubiKeyConnected",
                        ErrorCategory.InvalidOperation,
                        null));
                return;
            }

            try
            {
                // Factory reset the YubiKey
                YubiKeyModule._yubikey.DeviceReset();
                WriteInformation("YubiKey Bio MPE factory reset successful.", new string[] { "YubiKey", "Reset" });
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        ex,
                        "FactoryResetError",
                        ErrorCategory.OperationStopped,
                        null));
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}