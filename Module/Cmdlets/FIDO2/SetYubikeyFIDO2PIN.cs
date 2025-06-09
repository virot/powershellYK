/// <summary>
/// Sets or changes the FIDO2 PIN on a YubiKey.
/// Supports setting a new PIN or changing an existing PIN.
/// PIN length must be between 4 and 63 characters.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// $newPIN = ConvertTo-SecureString "123456" -AsPlainText -Force
/// Set-YubiKeyFIDO2PIN -NewPIN $newPIN
/// Sets a new FIDO2 PIN on a YubiKey that doesn't have a PIN set
/// 
/// .EXAMPLE
/// $oldPIN = ConvertTo-SecureString "123456" -AsPlainText -Force
/// $newPIN = ConvertTo-SecureString "654321" -AsPlainText -Force
/// Set-YubiKeyFIDO2PIN -OldPIN $oldPIN -NewPIN $newPIN
/// Changes the FIDO2 PIN on a YubiKey that already has a PIN set
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;
using System.Collections.ObjectModel;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Set, "YubiKeyFIDO2PIN")]
    public class SetYubikeyFIDO2PINCmdlet : PSCmdlet, IDynamicParameters
    {
        // Get dynamic parameters based on YubiKey state and capabilities
        public object GetDynamicParameters()
        {
            Collection<Attribute> oldPIN, newPIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Set minimum PIN length based on YubiKey capabilities
                    int minPinLength = fido2Session.AuthenticatorInfo.MinimumPinLength ?? 4;

                    // Configure old PIN parameter based on whether a PIN is already set
                    if (fido2Session.AuthenticatorInfo.Options!.Any(x => x.Key == AuthenticatorOptions.clientPin && x.Value == true) && (YubiKeyModule._fido2PIN is null))
                    {
                        oldPIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }
                    else
                    {
                        oldPIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }

                    // Configure new PIN parameter with minimum length requirement
                    newPIN = new Collection<Attribute>() {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO applet.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                        new ValidateYubikeyPIN(minPinLength, 63)
                    };
                }
            }
            else
            {
                // Default parameters when no YubiKey is connected
                oldPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
                newPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO applet.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
            }

            // Create and return dynamic parameters
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedOldPIN = new RuntimeDefinedParameter("OldPIN", typeof(SecureString), oldPIN);
            var runtimeDefinedNewPIN = new RuntimeDefinedParameter("NewPIN", typeof(SecureString), newPIN);
            runtimeDefinedParameterDictionary.Add("OldPIN", runtimeDefinedOldPIN);
            runtimeDefinedParameterDictionary.Add("NewPIN", runtimeDefinedNewPIN);
            return runtimeDefinedParameterDictionary;
        }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }

            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Get PIN parameters
                if (this.MyInvocation.BoundParameters.ContainsKey("OldPIN"))
                {
                    YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["OldPIN"];
                }
                YubiKeyModule._fido2PINNew = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];

                try
                {
                    // Set or change PIN based on current state
                    if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                    {
                        WriteDebug("No FIDO2 PIN set, setting new PIN...");
                        fido2Session.SetPin();
                    }
                    else
                    {
                        WriteDebug("FIDO2 PIN set, changing PIN...");
                        fido2Session.ChangePin();
                    }
                    WriteInformation("FIDO PIN updated.", new string[] { "FIDO2", "Info" });
                }
                catch (Exception e)
                {
                    YubiKeyModule._fido2PIN = null;
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    YubiKeyModule._fido2PINNew = null;
                }

                // Store new PIN
                YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
            }
        }
    }
}
