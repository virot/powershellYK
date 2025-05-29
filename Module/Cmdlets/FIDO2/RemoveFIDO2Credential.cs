/// <summary>
/// Removes a FIDO2 credential from a YubiKey.
/// Supports removing credentials by either their ID or by username and relying party.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// $cred = Get-YubiKeyFIDO2Credential | Select-Object -First 1
/// Remove-YubiKeyFIDO2Credential -CredentialId $cred.CredentialId
/// Removes a specific FIDO2 credential using its ID
/// 
/// .EXAMPLE
/// Remove-YubiKeyFIDO2Credential -Username "user@example.com" -RelayingParty "example.com"
/// Removes a FIDO2 credential by username and relying party
/// </summary>

// Imports
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
        // Parameters for credential identification
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential ID to remove", ParameterSetName = "Remove with CredentialID")]
        public powershellYK.FIDO2.CredentialID CredentialId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "User to remove", ParameterSetName = "Remove with username and RelayingParty")]
        public string Username { get; set; } = String.Empty;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelayingParty to remove user from", ParameterSetName = "Remove with username and RelayingParty")]
        public string RelayingParty { get; set; } = String.Empty;

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
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
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Enumerate all relying parties and their credentials
                var relyingParties = fido2Session.EnumerateRelyingParties();

                if (!relyingParties.Any())
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
