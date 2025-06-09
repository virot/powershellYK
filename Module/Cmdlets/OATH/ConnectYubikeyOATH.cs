/// <summary>
/// Connects to the OATH applet on a YubiKey.
/// Handles password authentication if the OATH applet is password protected.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Requires a YubiKey with OTP support.
/// 
/// .EXAMPLE
/// Connect-YubiKeyOATH
/// Connects to the OATH applet, prompting for password if needed
/// 
/// .EXAMPLE
/// $securePassword = ConvertTo-SecureString "P@ssw0rd" -AsPlainText -Force
/// Connect-YubiKeyOATH -Password $securePassword
/// Connects to the OATH applet using the provided password
/// </summary>

// Imports
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
        // Get dynamic parameters based on OATH state
        public object GetDynamicParameters()
        {
            Collection<Attribute> passwordAttributes;
            if (YubiKeyModule._yubikey is not null)
            {
                // Check password requirement for connected YubiKey
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
                // Try to connect to any available YubiKey
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
                    // Default to requiring password if no YubiKey is found
                    passwordAttributes = new Collection<Attribute>()
                    {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "Password provided as a SecureString.", ParameterSetName = "Password"},
                        new ValidateYubikeyPassword(0, 255)
                    };
                }
            }

            // Create and return dynamic parameters
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedPassword = new RuntimeDefinedParameter("Password", typeof(SecureString), passwordAttributes);
            runtimeDefinedParameterDictionary.Add("Password", runtimeDefinedPassword);
            return runtimeDefinedParameterDictionary;
        }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            try
            {
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    // Handle password authentication if required
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
