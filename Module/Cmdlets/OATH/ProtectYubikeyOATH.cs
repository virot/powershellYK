/// <summary>
/// Provides functionality to set or change the password protection for a YubiKey's OATH application.
/// This cmdlet allows users to either set an initial password on an unprotected OATH application
/// or change an existing password. Passwords can be provided as a parameter or entered interactively.
/// 
/// @Virot: it looks like theres a problem if the applet is not protected.
/// 
/// TEST CASES:
/// 1. Reset OATH applet then run Protect-YubikeyOATH (without parameter)
/// 2. Reset OATH applet then run Protect-YubikeyOATH with parameter
/// 3. Reset OATH applet then set password using Yubico Authenticator then run Cmlet
/// 4. ....
/// </summary>

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using powershellYK.support.validators;
using System.Security;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsSecurity.Protect, "YubiKeyOATH")]

    public class SetYubikeyOATH2Command : PSCmdlet
    {
        // Parameter for new password
        [ValidateYubikeyPassword(1, 255)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New password as a SecureString", ParameterSetName = "Change password")]
        public SecureString UpdatePassword = new SecureString();

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
                        // Prompt for existing password first
                        var myPowerShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace)
                            .AddCommand("Read-Host")
                            .AddParameter("AsSecureString")
                            .AddParameter("Prompt", "Enter current OATH Password");

                        // Store the password from prompt for use in password verification
                        YubiKeyModule._OATHPassword = (SecureString)myPowerShellInstance.Invoke()[0].BaseObject;

                        // Verify the current password from prompt before proceeding...
                        oathSession.VerifyPassword();
                    }

                    // Get new password - either from parameter or prompt
                    if (UpdatePassword.Length == 0)
                    {
                        WriteDebug("No password provided as parameter, prompting user.");
                        var newPasswordInstance = PowerShell.Create(RunspaceMode.CurrentRunspace)
                            .AddCommand("Read-Host")
                            .AddParameter("AsSecureString")
                            .AddParameter("Prompt", "Enter new OATH Password");

                        // Store the new password from prompt for use for use when setting the password
                        YubiKeyModule._OATHPasswordNew = (SecureString)newPasswordInstance.Invoke()[0].BaseObject;
                    }
                    else
                    {
                        WriteDebug("Using password provided as parameter.");
                        // Store the new password from parameter for use when setting the password
                        YubiKeyModule._OATHPasswordNew = UpdatePassword;
                    }

                    /*
                        oathSession.VerifyPassword();
                        YubiKeyModule._OATHPasswordNew = UpdatePassword;
                        oathSession.SetPassword();
                        YubiKeyModule._OATHPassword = UpdatePassword;
                        YubiKeyModule._OATHPasswordNew = null;
                    */

                    // If the OATH applet is NOT protected, set the password
                    if (!oathSession.IsPasswordProtected)
                    {
                        WriteDebug("Setting initial password on unprotected OATH applet.");
                        //oathSession.SetPassword(null, YubiKeyModule._OATHPasswordNew);
                        //YubiKeyModule._OATHPasswordNew = UpdatePassword;
                        oathSession.SetPassword();
                    }
                    else
                    {
                        WriteDebug("Changing existing password.");
                        //oathSession.SetPassword(YubiKeyModule._OATHPassword, YubiKeyModule._OATHPasswordNew);
                        oathSession.SetPassword();
                    }

                    YubiKeyModule._OATHPassword = YubiKeyModule._OATHPasswordNew;
                    YubiKeyModule._OATHPasswordNew = null;
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "OATHSessionError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}