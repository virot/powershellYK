using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Fido2.Commands;
using powershellYK.Exceptions;
using System.Diagnostics;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyFIDO2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyFIDO2Cmdlet : Cmdlet
    {
        // TODO: add more stuff here!

        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }

            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }
        }

        protected override void ProcessRecord()
        {
            if (ShouldProcess("", "WARNING!"))
            {
                // TODO: add credential removal logic here!
                
            }
        }

    }
}