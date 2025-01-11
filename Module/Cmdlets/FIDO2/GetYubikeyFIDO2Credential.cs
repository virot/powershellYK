using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;


namespace powershellYK.Cmdlets.Fido
{
    [Alias("Get-YubiKeyFIDO2Credentials")]
    [Cmdlet(VerbsCommon.Get, "YubiKeyFIDO2Credential")]

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
                var relyingParties = fido2Session.EnumerateRelyingParties();

                if (!relyingParties.Any()) // Check if there are no relying parties
                {
                    WriteWarning("No credentials found on the YubiKey.");
                    return;
                }
                else
                {
                    foreach (RelyingParty relyingParty in relyingParties)
                    {
                        WriteDebug($"Enumerating credentials for {relyingParty.Id}.");
                        IReadOnlyList<CredentialUserInfo> relayCredentials;
                        try
                        {
                            relayCredentials = fido2Session.EnumerateCredentialsForRelyingParty(relyingParty);
                        }
                        catch (NotSupportedException e)
                        {
                            WriteWarning($"Failed to enumerate credentials for {relyingParty.Id}: {e.Message}, SDK might not support algorithm.");
                            continue;
                        }

                        foreach (CredentialUserInfo user in relayCredentials)
                        {
                            Credential credential = new Credential(relyingParty: relyingParty, credentialUserInfo: user);
                            WriteObject(credential);
                        }
                    }
                }
            }
        }
    }
}