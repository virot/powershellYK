/// <summary>
/// Connects to a YubiKey PIV application.
/// Supports authentication with PIN and/or Management Key.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// Connect-YubiKeyPIV -PIN $pin
/// Connects to YubiKey PIV using PIN authentication
/// 
/// .EXAMPLE
/// Connect-YubiKeyPIV -ManagementKey $key -PIN $pin
/// Connects to YubiKey PIV using both Management Key and PIN
/// </summary>

// Imports
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
    [Cmdlet(VerbsCommunications.Connect, "YubiKeyPIV", DefaultParameterSetName = "PIN")]
    public class ConnectYubikeyPIVCommand : Cmdlet
    {
        // Parameter for the PIV Management Key
        [TransformHexInput()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Management Key", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Management Key", ParameterSetName = "Management")]
        public PSObject? ManagementKey;

        // Parameter for the PIV PIN
        [ValidateYubikeyPIN(6, 8)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN&Management")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN", ParameterSetName = "PIN")]
        public SecureString? PIN;

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Authenticate with PIN if provided
                if (PIN is not null)
                {
                    YubiKeyModule.setPIVPIN(PIN);
                    pivSession.VerifyPin();
                }

                // Authenticate with Management Key if provided
                if (ManagementKey is not null)
                {
                    YubiKeyModule._pivManagementKey = (byte[])ManagementKey.BaseObject;
                    pivSession.AuthenticateManagementKey();
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
