using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK;
using powershellYK.support.validators;
using System.ComponentModel.DataAnnotations;
using powershellYK.support.transform;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyPIV", DefaultParameterSetName = "PIN")]

    public class ConnectYubikeyPIVCommand : Cmdlet
    {
        [TransformHexInput()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ManagementKey", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ManagementKey", ParameterSetName = "Management")]
        public PSObject? ManagementKey;
        [ValidateYubikeyPIN(6, 8)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN")]
        public SecureString? PIN;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
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
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                if (PIN is not null)
                {
                    YubiKeyModule.setPIVPIN(PIN);
                    pivSession.VerifyPin();
                }
                if (ManagementKey is not null)
                {
                    YubiKeyModule._pivManagementKey = (byte[])ManagementKey.BaseObject;
                    pivSession.AuthenticateManagementKey();
                }
            }
        }
    }
}
