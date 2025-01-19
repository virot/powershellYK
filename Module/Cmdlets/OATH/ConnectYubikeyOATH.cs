// Summary:
// Connects to the OATH application on a YubiKey, handling password authentication if required.
// If no YubiKey is selected, it will automatically call Connect-Yubikey first.
//
// Examples:
// # Basic connection (will prompt for password if needed)
// Connect-YubiKeyOATH
//
// # Connect with password
// $securePassword = ConvertTo-SecureString "P@ssw0rd" -AsPlainText -Force
// Connect-YubiKeyOATH -Password $securePassword
//

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using powershellYK.support.validators;
using System.Security;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommunications.Connect, "YubiKeyOATH", DefaultParameterSetName = "Password")]
    public class ConnectYubikeyOATHCommand : PSCmdlet
    {
        [ValidateYubikeyPassword(0, 255)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Password provided as a SecureString", ParameterSetName = "Password")]
        public SecureString? Password;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    // Check if the OATH applet is password protected
                    if (oathSession.IsPasswordProtected)
                    {
                        WriteDebug("OATH applet is password protected.");
                        
                        if (Password == null) // Check if password parameter was used
                        {
                            // Prompt for password if not provided
                            var myPowerShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace)
                                .AddCommand("Read-Host")
                                .AddParameter("AsSecureString")
                                .AddParameter("Prompt", "Enter OATH Password");
                            
                            Password = (SecureString)myPowerShellInstance.Invoke()[0].BaseObject;
                        }

                        YubiKeyModule._OATHPassword = Password;
                        oathSession.VerifyPassword();
                        WriteDebug("Successfully authenticated and connected to OATH applet.");
                    }
                    else
                    {
                        WriteDebug("Successfully connected to OATH applet.");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "OATHSessionError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
