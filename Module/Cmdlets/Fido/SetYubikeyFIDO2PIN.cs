using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;
using System.Collections.ObjectModel;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Set, "YubikeyFIDO2PIN")]

    public class SetYubikeyFIDO2PINCmdlet : PSCmdlet, IDynamicParameters
    {
        public object GetDynamicParameters()
        {
            try { YubiKeyModule.ConnectYubikey(); } catch { }

            Collection<Attribute> oldPIN, newPIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // if no minimum pin length is set, then set it to 63.
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

                    newPIN = new Collection<Attribute>() {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO applet.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                        new ValidateYubikeyPIN(minPinLength, 63)
                    };
                }
            }
            else
            {
                oldPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = false, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
                newPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO applet.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
            }
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedOldPIN = new RuntimeDefinedParameter("OldPIN", typeof(SecureString), oldPIN);
            var runtimeDefinedNewPIN = new RuntimeDefinedParameter("NewPIN", typeof(SecureString), newPIN);
            runtimeDefinedParameterDictionary.Add("OldPIN", runtimeDefinedOldPIN);
            runtimeDefinedParameterDictionary.Add("NewPIN", runtimeDefinedNewPIN);
            return runtimeDefinedParameterDictionary;
        }

        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (this.MyInvocation.BoundParameters.ContainsKey("OldPIN"))
                {
                    YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["OldPIN"];
                }

                YubiKeyModule._fido2PINNew = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
                try
                {
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
                    WriteObject("FIDO PIN updated.");
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


            }
        }
    }
}
