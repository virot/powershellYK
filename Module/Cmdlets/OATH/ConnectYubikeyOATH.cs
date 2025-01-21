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
using powershellYK.support.validators;
using System.Security;
using System.Collections.ObjectModel;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommunications.Connect, "YubiKeyOATH", DefaultParameterSetName = "Password")]
    public class ConnectYubikeyOATHCommand : PSCmdlet, IDynamicParameters
    {
        public object GetDynamicParameters()
        {
            Collection<Attribute> passwordAttributes;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    if (oathSession.IsPasswordProtected)
                    {
                        passwordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(0, 255)
                        };
                    }
                    else
                    {
                        passwordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(0, 255)
                        };
                    }
                }
            }
            else
            {
                // Try to conect to any YubiKey that is inserted.
                try
                {
                    var yubiKey = YubiKeyDevice.FindAll().First();
                    using (var oathSession = new OathSession(yubiKey))
                    {
                        if (oathSession.IsPasswordProtected)
                        {
                            passwordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = true, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(0, 255)
                            };
                        }
                        else
                        {
                            passwordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = false, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(0, 255)
                            };
                        }
                    }
                }
                catch
                {
                    passwordAttributes = new Collection<Attribute>()
                    {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                        new ValidateYubikeyPassword(0, 255)
                    };
                }
            }
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedPassword = new RuntimeDefinedParameter("Password", typeof(SecureString), passwordAttributes);
            runtimeDefinedParameterDictionary.Add("Password", runtimeDefinedPassword);
            return runtimeDefinedParameterDictionary;
        }

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
                        try
                        {
                            YubiKeyModule._OATHPassword = (SecureString)this.MyInvocation.BoundParameters["Password"];
                            oathSession.VerifyPassword();
                            WriteDebug("Successfully authenticated and connected to OATH applet.");
                        }
                        catch (Exception ex)
                        {
                            YubiKeyModule._OATHPassword = null;
                            throw new SecurityException("Failed to authenticate with OATH applet. Please check the password and try again.", ex);
                        }
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
