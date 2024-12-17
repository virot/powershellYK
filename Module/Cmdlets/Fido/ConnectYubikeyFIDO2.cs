using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using System.Collections.ObjectModel;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommunications.Connect, "YubikeyFIDO2")]

    public class ConnectYubikeyFIDO2Command : PSCmdlet, IDynamicParameters
    {
        //Replaced with the dynamic one below.
        //[ValidateYubikeyPIN(4, 63)]
        //[Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIN")]
        //public SecureString PIN { get; set; } = new SecureString();

        public object GetDynamicParameters()
        {
            try { YubiKeyModule.ConnectYubikey(); } catch { }

            Collection<Attribute> PIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // if there is no PIN, dont require an entry of a PIN.
                    // We will use this and then force a reset using Set-YubikeyFIDO2 -SetPIN
                    if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.True)
                    {
                        PIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }
                    else
                    {
                        PIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }

                }
            }
            else
            {
                PIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
            }
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedPIN = new RuntimeDefinedParameter("PIN", typeof(SecureString), PIN);
            runtimeDefinedParameterDictionary.Add("PIN", runtimeDefinedPIN);
            return runtimeDefinedParameterDictionary;
        }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }

            // Check if Connect-YubikeyFIDO2 was called without a PIN (only possible with Yubikey that doesnt have a PIN configured)
            if (this.MyInvocation.BoundParameters.ContainsKey("PIN") == false)
            {
                WriteWarning("FIDO2 has no PIN, please set PIN before continuing.");
                WriteDebug("FIDO2 has no PIN, invokating Set-YubikeyFIDO2 -SetPIN");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Set-YubikeyFIDO2").AddParameter("SetPIN");
                myPowersShellInstance.Invoke();
            }


#if WINDOWS
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
#endif //WINDOWS
        }
        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("PIN"))
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                    {
                        WriteObject("Client PIN is not set");
                        return;
                    }
                    else if (fido2Session.AuthenticatorInfo.ForcePinChange == true)
                    {
                        WriteWarning("YubiKey requires PIN change to continue, see Set-YubikeyFIDO2 -SetPIN ");
                        return;
                    }
                    if (this.MyInvocation.BoundParameters["PIN"] is not null)
                    {
                        YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["PIN"];
                    }
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    fido2Session.VerifyPin();
                }
            }
        }
    }
}
