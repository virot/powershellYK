using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyPIV", DefaultParameterSetName = "PIN")]

    public class ConnectYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ManagementKey", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ManagementKey", ParameterSetName = "Management")]
        public string? ManagementKey;
        //Move validataion to here, I just could not get it to work.
        //[ValidateScript("$_.Length -ge 6 -and $_ -le 8", ErrorMessage = "PIN must be between 6 and 8 characters")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN")]
        public SecureString? PIN;

        protected override void BeginProcessing()
        {
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
                    YubiKeyModule._pivPIN = PIN;
                }
            }
            if (ManagementKey is not null)
            {
                YubiKeyModule._pivManagementKey = HexConverter.StringToByteArray(ManagementKey);
            }
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                pivSession.VerifyPin();
                pivSession.AuthenticateManagementKey();
            }
        }
    }
}