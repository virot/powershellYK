using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubikeyFIDO2Credentials")]

    public class GetYubikeyFIDO2CredentialsCommand : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            // If no FIDO2 PIN exists, we need to connect to the FIDO2 application
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

                var RelyingParties = fido2Session.EnumerateRelyingParties();

                if (!RelyingParties.Any()) // Check if there are no relying parties
                {
                    WriteWarning("No credentials found on the YubiKey.");
                    return;
                }
                else
                {
                    foreach (RelyingParty RelyingParty in RelyingParties)
                    {
                        WriteDebug($"Enumerating credentials for {RelyingParty.Id}.");
                        IReadOnlyList<CredentialUserInfo> relayCredentials;
                        try
                        {
                            relayCredentials = fido2Session.EnumerateCredentialsForRelyingParty(RelyingParty);
                        }
                        catch (NotSupportedException e)
                        {
                            WriteWarning($"Failed to enumerate credentials for {RelyingParty.Id}: {e.Message}, SDK might not support algorithm.");
                            continue;
                        }
                        foreach (CredentialUserInfo user in relayCredentials)
                        {
                            Credentials credentials = new Credentials
                            {
                                Site = RelyingParty.Id,
                                Name = user.User.Name,
                                DisplayName = user.User.DisplayName,
                                coseKey = user.CredentialPublicKey,
                            };
                            WriteObject(credentials);
                        }
                    }
                }
            }
        }

    }
}