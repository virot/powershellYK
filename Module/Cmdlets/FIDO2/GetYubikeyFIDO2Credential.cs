/// <summary>
/// Retrieves FIDO2 credentials stored on a YubiKey.
/// Lists all credentials or retrieves a specific credential by ID.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2Credential
/// Lists all FIDO2 credentials stored on the YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2Credential -CredentialID $credId
/// Retrieves a specific FIDO2 credential using its ID
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2Credential -CredentialIdBase64Url "base64url_encoded_id"
/// Retrieves a specific FIDO2 credential using its Base64URL encoded ID
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;

namespace powershellYK.Cmdlets.Fido
{
    [Alias("Get-YubiKeyFIDO2Credentials")]
    [Cmdlet(VerbsCommon.Get, "YubiKeyFIDO2Credential", DefaultParameterSetName = "List-All")]
    public class GetYubikeyFIDO2CredentialsCommand : PSCmdlet
    {
        // Parameters for credential filtering
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "List all", ParameterSetName = "List-All", DontShow = true)]
        public SwitchParameter All { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential ID to remove", ParameterSetName = "List-CredentialID")]
        public powershellYK.FIDO2.CredentialID? CredentialID { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential ID to remove int Base64 URL encoded format", ParameterSetName = "List-CredentialID-Base64URL")]
        public string? CredentialIdBase64Url { get; set; } = string.Empty;

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
            // Convert Base64URL credential ID if provided
            if (!this.CredentialID.HasValue && CredentialIdBase64Url is not null)
            {
                this.CredentialID = powershellYK.FIDO2.CredentialID.FromStringBase64URL(CredentialIdBase64Url);
            }

            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Enumerate all relying parties
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
                            if (ParameterSetName == "List-All" || (user.CredentialId.Id.ToArray().SequenceEqual(this.CredentialID!.Value.ToByte())))
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
}