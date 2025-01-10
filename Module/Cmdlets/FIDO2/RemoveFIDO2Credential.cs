using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using System.Linq;
using powershellYK.support;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyFIDO2Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Remove with CredentialID")]
    public class RemoveYubikeyFIDO2CredentialCmdlet : PSCmdlet
    {
        // Credential ID is required when calling the cmdlet.
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential ID to remove", ParameterSetName = "Remove with CredentialID")]
        public powershellYK.FIDO2.CredentialID CredentialId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "User to remove", ParameterSetName = "Remove with username and RelayingParty")]
        public string Username { get; set; } = String.Empty;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelayingParty to remove user from", ParameterSetName = "Remove with username and RelayingParty")]
        public string RelayingParty { get; set; } = String.Empty;

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

                // Since we cannot construct a CredentialID object, we need to find it. This unfortunately requires a full enumeration of all credentials.

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
                            if ((this.ParameterSetName == "Remove with CredentialID" && user.CredentialId.Id.ToArray().SequenceEqual(CredentialId.ToByte())) ||
                                (this.ParameterSetName == "Remove with username and RelayingParty" && Username == user.User.Name && RelayingParty == relyingParty.Id))
                            {
                                WriteDebug($"Found matching credentialID for: '{user.User.Name}' for '{relyingParty.Id}'.");
                                if (ShouldProcess($"This permanently remove credential for '{user.User.Name}' for '{relyingParty.Id}'. Credential ID: {new powershellYK.FIDO2.CredentialID(user.CredentialId)}", $"This permanently remove credential for '{user.User.Name}' for '{relyingParty.Id}'. Credential ID: {new powershellYK.FIDO2.CredentialID(user.CredentialId)}", "Warning"))
                                {
                                    try
                                    {
                                        fido2Session.DeleteCredential(user.CredentialId);
                                        WriteInformation("Credential removed.", new string[] { "FIDO2", "Info" });
                                        return;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception($"Failed to remove credential: {ex.Message}", ex);
                                    }
                                }
                            }
                            else
                            {
                                WriteDebug($"This didnt match.. wtf :D");
                            }
                        }
                    }
                    switch (this.ParameterSetName)
                    {
                        case "Remove with CredentialID":
                            throw new Exception("No credential found with the specified CredentialID.");
                        case "Remove with username and RelayingParty":
                            throw new Exception("No credential found with the specified Username / RelayingParty.");
                    }
                }
            }
        }
    }
}
