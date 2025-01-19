using powershellYK.Cmdlets.Other;
using powershellYK.support;
using System.Diagnostics;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Sample.PivSampleCode;

namespace powershellYK.Cmdlets.Bio
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyBioMPE", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetYubiKeyCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

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

        protected override void ProcessRecord()
        {
            if (!Force && !ShouldProcess("This will delete all stored data and restore factory settings. Proceed?", "This will delete all stored data and restore factory settings. Proceed?", "WARNING!"))
            {
                return;
            }

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