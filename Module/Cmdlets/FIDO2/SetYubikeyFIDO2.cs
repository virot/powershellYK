/// <summary>
/// Configures FIDO2 settings on a YubiKey.
/// Supports setting/changing PIN, minimum PIN length, and force PIN change.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// Note: Setting PIN is deprecated, use Set-YubiKeyFIDO2PIN instead.
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -SetPIN -NewPIN (ConvertTo-SecureString "123456" -AsPlainText -Force)
/// Sets a new FIDO2 PIN (deprecated, use Set-YubiKeyFIDO2PIN instead)
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -MinimumPINLength 8
/// Sets the minimum PIN length to 8 characters
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -ForcePINChange
/// Forces PIN change on next use
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -MinimumPINRelyingParty "example.com"
/// Sends minimum PIN length to specified relying party
/// </summary>

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;
using System.Collections.ObjectModel;
using Yubico.YubiKey.Piv;
using Microsoft.VisualBasic;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Set, "YubiKeyFIDO2")]
    public class SetYubikeyFIDO2Cmdlet : PSCmdlet, IDynamicParameters
    {
        // Parameters for PIN configuration
        [Parameter(Mandatory = false, ParameterSetName = "Set PIN", ValueFromPipeline = false, HelpMessage = "Easy access to Set new PIN")]
        public SwitchParameter SetPIN;

        [ValidateRange(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Set PIN minimum length", ValueFromPipeline = false, HelpMessage = "Set the minimum length of the PIN")]
        public int? MinimumPINLength { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Set force PIN change", HelpMessage = "Enable or disable the forceChangePin flag")]
        public SwitchParameter ForcePINChange { get; set; }

        [ValidateLength(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Send MinimumPIN to RelyingParty", ValueFromPipeline = false, HelpMessage = "To which RelyingParty should minimum PIN be sent")]
        public string? MinimumPINRelyingParty { get; set; }

        // Get dynamic parameters based on YubiKey state
        public object GetDynamicParameters()
        {
            Collection<Attribute> oldPIN, newPIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Set minimum PIN length based on YubiKey capabilities
                    int minPinLength = fido2Session.AuthenticatorInfo.MinimumPinLength ?? 4;
                    // Verify that the yubikey FIDO2 has a PIN already set. If there is a PIN set then make sure we get the old PIN.
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

                    // Configure new PIN parameter with minimum length
                    newPIN = new Collection<Attribute>() {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO2 module.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                        new ValidateYubikeyPIN(minPinLength, 63)
                    };
                }
            }
            else
            {
                // Default parameters when no YubiKey is connected
                oldPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = false, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
                newPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO2 module.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
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
            // Connect to appropriate interface based on operation
            if (ParameterSetName == "Set PIN")
            {
                // Connect to YubiKey if not already connected
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey...");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
            }
            else
            {
                // Connect to FIDO2 if not already authenticated
                if (YubiKeyModule._fido2PIN is null)
                {
                    WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2...");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    if (YubiKeyModule._fido2PIN is null)
                    {
                        throw new Exception("Connect-YubikeyFIDO2 failed to connect to the FIDO2 applet!");
                    }
                }
            }

            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                switch (ParameterSetName)
                {
                    case "Set PIN minimum length":
                        // Verify support for minimum PIN length
                        if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.setMinPINLength) == OptionValue.True)
                        {
                            // Set minimum PIN length
                            if (!fido2Session.TrySetPinConfig(MinimumPINLength, null, null))
                            {
                                throw new Exception("Failed to change the minimum PIN length.");
                            }
                            // Force PIN change
                            fido2Session.TrySetPinConfig(null, null, null);
                            WriteInformation("Minimum PIN length set.", new string[] { "FIDO2", "Info" });
                        }
                        else
                        {
                            throw new Exception("Changing minimum PIN is not supported in this YubiKey firmware version.");
                        }
                        break;

                    // Set force PIN change will expire the PIN on initial use forcing the user to set a new PIN.
                    case "Set force PIN change":
                        // Verify support for force PIN change
                        if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.setMinPINLength) == OptionValue.True)
                        {
                            // Enable force PIN change
                            bool? forceChangePin = true;
                            if (fido2Session.TrySetPinConfig(null, null, forceChangePin))
                            {
                                WriteInformation("Force PIN Change set.", new string[] { "FIDO2", "Info" });
                            }
                            else
                            {
                                // Throw an exception if applying the setting fails.
                                throw new InvalidOperationException("Failed to enforce PIN change.");
                            }
                        }
                        else
                        {
                            // Throw an exception if the hardware does not support the feature.
                            throw new NotSupportedException("Forcing PIN change is not supported in this YubiKey firmware version.");
                        }
                        break;

                    case "Set PIN":
                        // Handle PIN setting/changing
                        if (this.MyInvocation.BoundParameters.ContainsKey("OldPIN"))
                        {
                            YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["OldPIN"];
                        }

                        YubiKeyModule._fido2PINNew = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
                        try
                        {
                            if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                            {
                                WriteDebug("No FIDO PIN set, setting new PIN...");
                                fido2Session.SetPin();
                            }
                            else
                            {
                                WriteDebug("FIDO2 PIN set, changing PIN...");
                                fido2Session.ChangePin();
                            }
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
                        YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
                        WriteWarning("Changing FIDO2 PIN using Set-YubiKeyFIDO2 has been deprecated, use Set-YubiKeyFIDO2PIN instead.");
                        WriteInformation("FIDO PIN updated.", new string[] { "FIDO2", "Info" });
                        break;

                    case "Send MinimumPIN to RelyingParty":
                        // Send minimum PIN length to relying party
                        var rpidList = new List<string>(1);
                        RelyingParty rp = new RelyingParty(MinimumPINRelyingParty!);
                        rpidList.Add(rp.Id);
                        if (!fido2Session.TrySetPinConfig(null, rpidList, null))
                        {
                            throw new Exception("Failed to set RelyingParty that will be sent Minimum PIN length.");
                        }
                        break;
                }
            }
        }
    }
}
