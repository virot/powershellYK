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
    [Cmdlet(VerbsSecurity.Unprotect, "YubikeyOATH")]

    public class UnprotectYubikeyOATH2Command : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Remove password requirement", ParameterSetName = "Clear password")]
        public SwitchParameter ClearPassword;
        [ValidateYubikeyPassword(1, 255)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Password", ParameterSetName = "Change password")]
        public SecureString UpdatePassword = new SecureString();


        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (oathSession.IsPasswordProtected)
                {
                    oathSession.UnsetPassword();
                    YubiKeyModule._OATHPassword = null;
                }
            }
        }
    }
}
