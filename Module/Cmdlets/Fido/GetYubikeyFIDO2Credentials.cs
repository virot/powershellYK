using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubikeyFIDO2Credentials")]

    public class GetYubikeyFIDO2CredentialsCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
            }
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("You need to run this command as an administrator.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                var RelyingParties = fido2Session.EnumerateRelyingParties();
                foreach (RelyingParty RelyingParty in RelyingParties)
                {
                    var relayCredentials = fido2Session.EnumerateCredentialsForRelyingParty(RelyingParty);
                    foreach (CredentialUserInfo user in relayCredentials)
                    {
                        Credentials credentials = new Credentials
                        {
                            Site = RelyingParty.Id,
                            Name = user.User.Name,
                            DisplayName = user.User.DisplayName,
                        };
                        WriteObject(credentials);
                    }
                }
            }
        }
    }
}
