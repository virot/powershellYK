using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using VirotYubikey.support;
using System.Data.Common;

namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyPIV")]

    public class ConnectYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "ManagementKey")]
        public string ManagementKey { get; set; } = "010203040506070801020304050607080102030405060708";
        [ValidateLength(6, 8)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "PIN")]
        public string PIN { get; set; } = "123456";

        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._pivSession is not null)
            {
                WriteDebug("Disconnecting old session");
                YubiKeyModule._pivSession.Dispose();
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
                WriteDebug("Connecting to Yubikey PIV Session");
                YubiKeyModule._pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey PIV Session", e);
            }
            try
            {
                byte[] mgmtKey = HexConverter.StringToByteArray(ManagementKey);
                if (YubiKeyModule._pivSession.TryAuthenticateManagementKey(mgmtKey, true)) {
                }
                else
                {
                    throw new Exception("Could not authenticate to YubiKey");
                }
                WriteDebug("Initial Authentication");
                CryptographicOperations.ZeroMemory(mgmtKey);
            }
            catch (Exception e) {
                new Exception("Could not authenticate Yubikey PIV, wrong ManagementKey", e);
            }

            try
            {
                byte[] pinarray = System.Text.Encoding.UTF8.GetBytes(PIN);
                int? retries = null;
                if (YubiKeyModule._pivSession.TryVerifyPin(pinarray, out retries) == false)
                {
                    throw new Exception("Could not authenticate Yubikey PIV, wrong PIN");
                }
                CryptographicOperations.ZeroMemory(pinarray);
            }
            catch (Exception e)
            {
                throw new Exception("Could not authenticate Yubikey PIV, wrong PIN", e);
            }

        }
    }
}