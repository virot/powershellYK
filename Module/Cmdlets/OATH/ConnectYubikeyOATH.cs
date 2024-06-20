using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using powershellYK.support.validators;
using System.Security;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyOATH",DefaultParameterSetName = "Password")]

    public class ConnectYubikeyOATHCommand : Cmdlet
    {
        [ValidateYubikeyPassword(0, 255)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Password", ParameterSetName = "Password")]
        public SecureString? Password;
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
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

                if (oathSession.IsPasswordProtected && Password is not null && Password.Length >= 1)
                {
                    YubiKeyModule._OATHPassword = Password;
                    oathSession.VerifyPassword();
                }
            }
        }
    }
}