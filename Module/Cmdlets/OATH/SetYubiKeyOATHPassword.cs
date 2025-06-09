/// <summary>
/// Sets or changes password protection for the YubiKey OATH applet.
/// Allows setting an initial password or changing an existing one.
/// Requires a YubiKey with OTP support.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Note: This operation cannot be undone.
/// 
/// .EXAMPLE
/// Set-YubiKeyOATHPassword
/// Sets a new password interactively
/// 
/// .EXAMPLE
/// $pass = ConvertTo-SecureString "password" -AsPlainText -Force
/// Set-YubiKeyOATHPassword -NewPassword $pass
/// Sets a new password using a SecureString
/// </summary>

// Imports
using System.Management.Automation;           // PowerShell cmdlet base classes and attributes
using Yubico.YubiKey;                        // YubiKey device management and session handling
using Yubico.YubiKey.Oath;                   // OATH application functionality and credential management
using powershellYK.support.validators;       // Custom parameter validation for YubiKey operations
using System.Security;                       // SecureString for password handling
using System.Collections.ObjectModel;        // Collection types for dynamic parameter attributes

namespace powershellYK.Cmdlets.OATH
{
    // Alias for backward compatibility with Protect-YubiKeyOATH command
    [Alias("Protect-YubiKeyOATH")]
    [Cmdlet(VerbsCommon.Set, "YubiKeyOATHPassword")]
    public class SetYubiKeyOATHPasswordCmdlet : PSCmdlet, IDynamicParameters
    {
        // Dynamic parameter handling for password management
        public object GetDynamicParameters()
        {
            // Configure new password parameter with validation
            Collection<Attribute> newPasswordAttributes = new Collection<Attribute>()
            {
                new ParameterAttribute() { Mandatory = true, HelpMessage = "New password provided as a SecureString.", ParameterSetName = "Password"},
                new ValidateYubikeyPassword(1, 255)
            };

            // Configure old password parameter based on current state
            Collection<Attribute> OldPasswordAttributes;
            if (YubiKeyModule._yubikey is not null)
            {
                // Check current YubiKey state
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Require old password if applet is protected and no password is cached
                    if (oathSession.IsPasswordProtected && YubiKeyModule._OATHPassword is null)
                    {
                        OldPasswordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(1, 255)
                        };
                    }
                    else
                    {
                        // Make old password optional if applet is not protected or password is cached
                        OldPasswordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(1, 255)
                        };
                    }
                }
            }
            else
            {
                // Try to connect to any YubiKey that is inserted
                try
                {
                    var yubiKey = YubiKeyDevice.FindAll().First();
                    using (var oathSession = new OathSession(yubiKey))
                    {
                        // Require old password if applet is protected and no password is cached
                        if (oathSession.IsPasswordProtected && YubiKeyModule._OATHPassword is null)
                        {
                            OldPasswordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(1, 255)
                            };
                        }
                        else
                        {
                            // Make old password optional if applet is not protected or password is cached
                            OldPasswordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = false, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(1, 255)
                            };
                        }
                    }
                }
                catch
                {
                    // Default to requiring old password if connection fails
                    OldPasswordAttributes = new Collection<Attribute>()
                    {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                        new ValidateYubikeyPassword(1, 255)
                    };
                }
            }

            // Create and return dynamic parameters
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedOldPassword = new RuntimeDefinedParameter("OldPassword", typeof(SecureString), OldPasswordAttributes);
            var runtimeDefinedNewPassword = new RuntimeDefinedParameter("NewPassword", typeof(SecureString), newPasswordAttributes);
            runtimeDefinedParameterDictionary.Add("OldPassword", runtimeDefinedOldPassword);
            runtimeDefinedParameterDictionary.Add("NewPassword", runtimeDefinedNewPassword);
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
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for password handling
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                try
                {
                    // Set old password if provided in parameters
                    if (this.MyInvocation.BoundParameters.ContainsKey("OldPassword"))
                    {
                        YubiKeyModule._OATHPassword = (SecureString)this.MyInvocation.BoundParameters["OldPassword"];
                    }

                    // Set new password from parameters
                    YubiKeyModule._OATHPasswordNew = (SecureString)this.MyInvocation.BoundParameters["NewPassword"];

                    // Apply password change to YubiKey
                    oathSession.SetPassword();

                    // Update cached password and clear temporary storage
                    YubiKeyModule._OATHPassword = YubiKeyModule._OATHPasswordNew;
                    YubiKeyModule._OATHPasswordNew = null;
                    WriteInformation("YubiKey OATH applet password set.", new string[] { "OATH", "Info" });
                }
                catch (Exception ex)
                {
                    // Clear cached passwords on error to prevent stale state
                    YubiKeyModule._OATHPassword = null;
                    YubiKeyModule._OATHPasswordNew = null;
                    WriteError(new ErrorRecord(ex, "Failed to update password for OATH applet", ErrorCategory.OperationStopped, null));
                }
            }
        }
    }
}