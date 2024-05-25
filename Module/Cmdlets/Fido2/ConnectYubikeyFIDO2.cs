using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using VirotYubikey.support;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace VirotYubikey.Cmdlets.Fido2
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyFIDO2")]

    public class ConnectYubikeyFIDO2Command : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "ManagementKey")]
        public string ManagementKey { get; set; } = "010203040506070801020304050607080102030405060708";
        [ValidateLength(6, 8)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "PIN")]
        public string PIN { get; set; } = "123456";

        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._fido2Session is not null)
            {
                WriteDebug("Disconnecting old session");
                YubiKeyModule._fido2Session.Dispose();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PermisionsStuff permisionsStuff = new PermisionsStuff();
                if (PermisionsStuff.IsRunningAsAdministrator() == false)
                {
                    throw new Exception("You need to run this command as an administrator");
                }
            }
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
                YubiKeyModule._fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey FIDO2", e);
            }

        }
    }
}