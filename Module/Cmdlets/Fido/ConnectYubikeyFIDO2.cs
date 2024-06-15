using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyFIDO2")]

    public class ConnectYubikeyFIDO2Command : Cmdlet
    {
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
                    WriteDebug($"Successfully connected");
                }
            }
#if WINDOWS
            PermisionsStuff permisionsStuff = new PermisionsStuff();
            if (PermisionsStuff.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator");
            }
#endif //WINDOWS
        }
        protected override void ProcessRecord()
        {
            if (PIN is not null)
            {
                if (PIN.Length < 6 || PIN.Length > 8)
                {
                    throw new ArgumentException("PIN must be between 6 and 8 characters");
                }
                else
                {
                    YubiKeyModule._fido2PIN = PIN;
                }
            }
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                fido2Session.VerifyPin();
            }
        }
    }
}