using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubikey_Powershell;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using Yubikey_Powershell.support;
using System.Data.Common;

namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyPIV")]

    public class ConnectYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "ManagementKey")]
        public string ManagementKey { get; set; } = "010203040506070801020304050607080102030405060708";
        [ValidateLength(6, 8)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "PIN")]
        public string PIN { get; set; } = "123456";

        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._pivSession is not null) { YubiKeyModule._pivSession.Dispose(); }
            if (YubiKeyModule._yubikey is null) { throw new Exception("No YubiKey connected, use Connect-YubiKey first"); }
            try
            {
                YubiKeyModule._pivSession = new PivSession(YubiKeyModule._yubikey);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey PIV", e);
            }
            try
            {
                byte[] mgmtKey = HexConverter.StringToByteArray(ManagementKey);
                if (YubiKeyModule._pivSession.TryAuthenticateManagementKey(mgmtKey, true)) { } else
                {
                    throw new Exception("Could not authenticate to YubiKey");
                }
                WriteDebug("Initial Authentication");
                CryptographicOperations.ZeroMemory(mgmtKey);
            }
            catch (Exception e) { new Exception("Could not authenticate Yubikey PIV, wrong ManagementKey", e); }

            byte[] pinarray = System.Text.Encoding.UTF8.GetBytes(PIN);
            int? retries = null;
            YubiKeyModule._pivSession.TryVerifyPin(pinarray, out retries);
            CryptographicOperations.ZeroMemory(pinarray);
   
        }
       }
}