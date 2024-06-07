using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyOATH")]

    public class ConnectYubikeyOATHCommand : Cmdlet
    {

        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._oathSession is not null)
            {
                WriteDebug("Disconnecting old session");
                YubiKeyModule._oathSession.Dispose();
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
                WriteDebug("Connecting to Yubikey OATH Session");
                YubiKeyModule._oathSession  = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey OATH", e);
            }
        }
    }
}