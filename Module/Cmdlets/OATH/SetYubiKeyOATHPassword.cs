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
using System.Collections.ObjectModel;

namespace powershellYK.Cmdlets.OATH
{
    [Alias("Protect-YubiKeyOATH")]
    [Cmdlet(VerbsCommon.Set, "YubiKeyOATHPassword")]
    public class SetYubiKeyOATHPasswordCmdlet : PSCmdlet, IDynamicParameters
    {
        public object GetDynamicParameters()
        {
            Collection<Attribute> newPasswordAttributes = new Collection<Attribute>()
            {
                new ParameterAttribute() { Mandatory = true, HelpMessage = "New password provided as a SecureString.", ParameterSetName = "Password"},
                new ValidateYubikeyPassword(1, 255)
            };
            Collection<Attribute> OldPasswordAttributes;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    if (oathSession.IsPasswordProtected && YubiKeyModule._OATHPassword is null)
                    {
                        OldPasswordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(1, 255)
                        };
                    }
                    else
                    {
                        OldPasswordAttributes = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                            new ValidateYubikeyPassword(1, 255)
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
                        if (oathSession.IsPasswordProtected && YubiKeyModule._OATHPassword is null)
                        {
                            OldPasswordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(1, 255)
                            };
                        }
                        else
                        {
                            OldPasswordAttributes = new Collection<Attribute>() {
                                new ParameterAttribute() { Mandatory = false, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                                new ValidateYubikeyPassword(1, 255)
                            };
                        }
                    }
                }
                catch
                {
                    OldPasswordAttributes = new Collection<Attribute>()
                    {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "Current password provided as a SecureString.", ParameterSetName = "Password"},
                        new ValidateYubikeyPassword(1, 255)
                    };
                }
            }
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedOldPassword = new RuntimeDefinedParameter("OldPassword", typeof(SecureString), OldPasswordAttributes);
            var runtimeDefinedNewPassword = new RuntimeDefinedParameter("NewPassword", typeof(SecureString), newPasswordAttributes);
            runtimeDefinedParameterDictionary.Add("OldPassword", runtimeDefinedOldPassword);
            runtimeDefinedParameterDictionary.Add("NewPassword", runtimeDefinedNewPassword);
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
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                // Check if the OATH applet is password protected
                try
                {
                    if (this.MyInvocation.BoundParameters.ContainsKey("OldPassword"))
                    {
                        YubiKeyModule._OATHPassword = (SecureString)this.MyInvocation.BoundParameters["OldPassword"];
                    }
                    YubiKeyModule._OATHPasswordNew = (SecureString)this.MyInvocation.BoundParameters["NewPassword"];

                    oathSession.SetPassword();
                    YubiKeyModule._OATHPassword = YubiKeyModule._OATHPasswordNew;
                    YubiKeyModule._OATHPasswordNew = null;
                    WriteInformation("YubiKey OATH applet password set.", new string[] { "OATH", "Info" });
                }
                catch (Exception ex)
                {
                    YubiKeyModule._OATHPassword = null;
                    YubiKeyModule._OATHPasswordNew = null;
                    WriteError(new ErrorRecord(ex, "Failed to update password for OATH applet", ErrorCategory.OperationStopped, null));
                }


            }
        }
    }
}