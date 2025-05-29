/// <summary>
/// Connects to the FIDO2 application on a YubiKey.
/// Handles PIN authentication if required and verifies FIDO2 capabilities.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// Connect-YubiKeyFIDO2
/// Connects to FIDO2 on a YubiKey that doesn't have a PIN set
/// 
/// .EXAMPLE
/// $pin = ConvertTo-SecureString "123456" -AsPlainText -Force
/// Connect-YubiKeyFIDO2 -PIN $pin
/// Connects to FIDO2 on a YubiKey that has a PIN set
/// </summary>

// Imports
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
    [Cmdlet(VerbsCommunications.Connect, "YubiKeyFIDO2")]
    public class ConnectYubikeyFIDO2Command : PSCmdlet, IDynamicParameters
    {
        // Get dynamic parameters based on YubiKey state
        public object GetDynamicParameters()
        {
            try { YubiKeyModule.ConnectYubikey(); } catch { }

            Collection<Attribute> PIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Configure PIN parameter based on whether a PIN is set
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

            // Create and return dynamic parameters
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedPIN = new RuntimeDefinedParameter("PIN", typeof(SecureString), PIN);
            runtimeDefinedParameterDictionary.Add("PIN", runtimeDefinedPIN);
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
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }

            // Check if Connect-YubikeyFIDO2 was called without a PIN (only possible with Yubikey that doesnt have a PIN configured)
            /*
            if (this.MyInvocation.BoundParameters.ContainsKey("PIN") == false)
            {
                WriteWarning("FIDO2 has no PIN, please set PIN before continuing:");
                WriteDebug("FIDO2 has no PIN, invokating Set-YubikeyFIDO2 -SetPIN...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Set-YubikeyFIDO2").AddParameter("SetPIN");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
            }
            */


#if WINDOWS
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
#endif //WINDOWS
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("PIN"))
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Check FIDO2 PIN status
                    if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                    {
                        WriteWarning("Client PIN is not set, see Set-YubiKeyFIDO2PIN.");
                        return;
                    }
                    else if (fido2Session.AuthenticatorInfo.ForcePinChange == true)
                    {
                        WriteWarning("YubiKey requires PIN change to continue, see Set-YubiKeyFIDO2PIN.");
                        return;
                    }

                    // Store and verify PIN
                    if (this.MyInvocation.BoundParameters["PIN"] is not null)
                    {
                        YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["PIN"];
                    }
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    try
                    {
                        fido2Session.TryVerifyPin();
                    }
                    catch
                    {
                        YubiKeyModule._fido2PIN = null;
                        throw new UnauthorizedAccessException("Invalid PIN");
                    }
                }
            }
        }
    }
}
