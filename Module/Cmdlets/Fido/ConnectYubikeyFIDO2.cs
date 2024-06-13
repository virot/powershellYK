using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyFIDO2")]

    public class ConnectYubikeyFIDO2Command : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN")]
        public SecureString PIN { get; set; } = new SecureString();

        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._fido2Session is not null)
            {
                WriteDebug("Disconnecting old session");
                YubiKeyModule._fido2Session.Dispose();
            }
#if WINDOWS
                PermisionsStuff permisionsStuff = new PermisionsStuff();
                if (PermisionsStuff.IsRunningAsAdministrator() == false)
                {
                    throw new Exception("You need to run this command as an administrator");
                }
#endif //WINDOWS
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            try
            {
                WriteDebug("Connecting to Yubikey FIDO2 Session");
                YubiKeyModule._fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey FIDO2", e);
            }

            try
            {
                int? retries = null;
                bool? rebootRequired;
                if (YubiKeyModule._fido2Session.TryVerifyPin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PIN))!), null, null, out retries, out rebootRequired) == false)
                {
                    throw new Exception("Could not authenticate Yubikey PIV, wrong PIN");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not authenticate to YubiKey FIDO", e);
            }
        }
    }
}