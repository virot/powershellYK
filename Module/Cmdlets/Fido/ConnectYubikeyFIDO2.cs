using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyFIDO2")]

    public class ConnectYubikeyFIDO2Command : Cmdlet
    {
        [ValidateYubikeyPIN(4, 63)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN")]
        public SecureString PIN { get; set; } = new SecureString();

        protected override void BeginProcessing()
        {
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
            }
#if WINDOWS
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator");
            }
#endif //WINDOWS
        }
        protected override void ProcessRecord()
        {
            if (PIN is not null)
            {
                YubiKeyModule._fido2PIN = PIN;
            }
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                fido2Session.VerifyPin();
            }
        }
    }
}
