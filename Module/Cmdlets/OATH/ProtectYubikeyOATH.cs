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
    [Cmdlet(VerbsSecurity.Protect, "YubikeyOATH")]

    public class SetYubikeyOATH2Command : PSCmdlet
    {
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
                WriteDebug($"Successfully connected");
            }
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                oathSession.VerifyPassword();
                YubiKeyModule._OATHPasswordNew = UpdatePassword;
                oathSession.SetPassword();
                YubiKeyModule._OATHPassword = UpdatePassword;
                YubiKeyModule._OATHPasswordNew = null;
            }
        }
    }
}
