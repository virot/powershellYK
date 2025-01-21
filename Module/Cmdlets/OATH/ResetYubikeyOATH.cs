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
            if (ShouldProcess("This will delete all OATH credentials, and restore factory settings. Proceed?", "This will delete all OATH credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                try
                {
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

