using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Set, "YubikeyFIDO2")]

    public class SetYubikeyFIDO2Cmdlet : PSCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "Set new PIN", ValueFromPipeline = false, HelpMessage = "Easy access to Set new PIN")]
        public SwitchParameter SetPIN;
        [ValidateYubikeyPIN(4,8)]
        [Parameter(Mandatory = true, ParameterSetName = "Set new PIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        public SecureString NewPIN { get; set; } = new SecureString();
        [ValidateRange(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Set PIN minimum length", ValueFromPipeline = false, HelpMessage = "Set the minimum length of the PIN")]
        public int? MinimumPINLength { get; set; }
        [ValidateLength(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Send MinimumPIN to RelyingParty", ValueFromPipeline = false, HelpMessage = "To which RelyingParty should minimum PIN be sent")]
        public string? MinimumPINRelyingParty { get; set; }


        protected override void BeginProcessing()
        {
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
            }
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                switch (ParameterSetName)
                {
                    case "Set PIN minimum length":
                        if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.setMinPINLength) == OptionValue.True)
                        {
                            // Code to increase min PIN length here.

                            if (!fido2Session.TrySetPinConfig(MinimumPINLength, null, null))
                            {
                                throw new Exception("Failed to change the minimum PIN length..");
                            }
                            // Do it once more to force PIN change.
                            fido2Session.TrySetPinConfig(null, null, null);
                        }
                        else
                        {
                            throw new Exception("Changing minimum PIN not possible with this yubikey hardware.");
                        }
                        break;
                    case "Set new PIN":
                        YubiKeyModule._fido2PINNew = NewPIN;
                        try
                        {
                            if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                            {
                                WriteDebug("No FIDO2 PIN set, setting new PIN");
                                fido2Session.SetPin();
                            }
                            else
                            {
                                WriteDebug("FIDO2 PIN set, changing PIN");
                                fido2Session.ChangePin();
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message, e);
                        }
                        finally
                        {
                            YubiKeyModule._fido2PINNew = null;
                        }
                        YubiKeyModule._fido2PIN = NewPIN;
                        break;
                    case "Send MinimumPIN to RelyingParty":
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