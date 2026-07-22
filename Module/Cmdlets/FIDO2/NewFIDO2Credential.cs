/// <summary>
/// Creates a new FIDO2 discoverable credential on a YubiKey.
/// Supports creating credentials with various parameters including Relying Party (RP) 
/// information, user data, and authentication options. Requires a YubiKey with FIDO2 support 
/// and administrator privileges on Windows. When used with only -RelyingPartyID and -Username 
/// (Synthetic parameter set), the cmdlet auto-generates a cryptographic challenge and 
/// random user ID so no external IdP is needed.
/// 
/// .EXAMPLE
/// New-YubiKeyFIDO2Credential -RelyingPartyID "example.local" -Username "alice@example.local"
/// Creates a synthetic credential (without an actual IdP) with a default display name.
/// 
/// .EXAMPLE
/// New-YubiKeyFIDO2Credential -RelyingPartyID "example.local" -Username "alice@example.local" -UserDisplayName "Alice Smith"
/// Creates a synthetic credential (without an actual IdP) with a custom display name.
/// 
/// .EXAMPLE
/// $challengeB64Url = "&lt;challenge from relying party registerBegin response&gt;"
/// $challenge = [powershellYK.FIDO2.Challenge]::FromBase64URLEncoded($challengeB64Url)
/// New-YubiKeyFIDO2Credential -RelyingPartyID "example.com" -RelyingPartyName "Example" -Username "user@example.com" -UserID ([byte[]](0x01)) -Challenge $challenge
/// Creates a credential using the challenge issued by the relying party during registration.
/// 
/// .EXAMPLE
/// $rp = Get-YubiKeyFIDO2Credential | Select-Object -First 1 -ExpandProperty RelyingParty
/// $challenge = [powershellYK.FIDO2.Challenge]::CreateSyntheticChallenge($rp.Id)
/// New-YubiKeyFIDO2Credential -RelyingParty $rp -Username "user@example.com" -UserID ([byte[]](0x01)) -Challenge $challenge
/// Creates a credential reusing a relying party from an existing credential with a locally generated challenge.
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using Yubico.YubiKey.Cryptography;
using powershellYK.FIDO2;
using powershellYK.support.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.New, "YubiKeyFIDO2Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubikeyFIDO2CredentialCmdlet : PSCmdlet
    {
        // Parameters for relying party information
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Specify which relayingParty (site) this credential is regards to.", ParameterSetName = "UserData-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Specify which relayingParty (site) this credential is regards to.", ParameterSetName = "UserEntity-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Relying party ID (domain) for the credential.", ParameterSetName = "Synthetic")]
        public required string RelyingPartyID { private get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Friendlyname for the relayingParty.", ParameterSetName = "UserData-HostData")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Friendlyname for the relayingParty.", ParameterSetName = "UserEntity-HostData")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Friendly name for the relying party. Defaults to RelyingPartyID.", ParameterSetName = "Synthetic")]
        public required string RelyingPartyName { private get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelaingParty object.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelaingParty object.", ParameterSetName = "UserEntity-RelyingParty")]
        public required RelyingParty RelyingParty { private get; set; }

        // Parameters for user information
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Username to create credental for.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Username to create credental for.", ParameterSetName = "UserData-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Username for the credential.", ParameterSetName = "Synthetic")]
        public required string Username { private get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "UserDisplayName to create credental for.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "UserDisplayName to create credental for.", ParameterSetName = "UserData-HostData")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Display name for the user. Defaults to Username.", ParameterSetName = "Synthetic")]
        public string? UserDisplayName { private get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "UserID.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "UserID.", ParameterSetName = "UserData-HostData")]
        public byte[]? UserID { private get; set; }

        // Parameters for credential configuration
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challenge for credential registration.", ParameterSetName = "UserData-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challenge for credential registration.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challenge for credential registration.", ParameterSetName = "UserEntity-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challenge for credential registration.", ParameterSetName = "UserEntity-RelyingParty")]
        public Challenge? Challenge { private get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Should this credential be discoverable.")]
        public bool Discoverable { private get; set; } = true;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Supply the user entity in complete form.", ParameterSetName = "UserEntity-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Supply the user entity in complete form.", ParameterSetName = "UserEntity-RelyingParty")]
        public UserEntity? UserEntity { get; set; } = new UserEntity(new byte[] { 0, 0 });

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Algorithms the RelyingParty accepts")]
        public List<Yubico.YubiKey.Fido2.Cose.CoseAlgorithmIdentifier> RequestedAlgorithms = new List<Yubico.YubiKey.Fido2.Cose.CoseAlgorithmIdentifier> { Yubico.YubiKey.Fido2.Cose.CoseAlgorithmIdentifier.ES256 };

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

                if (ParameterSetName == "Synthetic")
                {
                    WriteDebug("Synthetic mode: generating Challenge and UserID automatically.");
                    Challenge = FIDO2.Challenge.CreateSyntheticChallenge(RelyingPartyID);
                    RelyingParty = new RelyingParty(RelyingPartyID) { Name = RelyingPartyName ?? RelyingPartyID };
                    byte[] syntheticUserId = SyntheticCredentialHelper.GenerateUserID();
                    WriteDebug($"Generated synthetic UserID: {Converter.ByteArrayToString(syntheticUserId)}");
                    UserEntity = new UserEntity(syntheticUserId.AsMemory())
                    {
                        Name = Username,
                        DisplayName = UserDisplayName ?? Username,
                    };
                }

                // Configure relying party information
                if (RelyingParty is null)
                {
                    WriteDebug($"Building new RelyingParty with {RelyingPartyID} and {RelyingPartyName ?? RelyingPartyID}");
                    RelyingParty = new RelyingParty(RelyingPartyID) { Name = RelyingPartyName ?? RelyingPartyID };
                }

                // Configure user entity information
                if ((ParameterSetName == "UserData-HostData" || ParameterSetName == "UserData-RelyingParty"))
                {
                    if (UserID is not null)
                    {
                        WriteDebug($"Building new UserEntity with {Converter.ByteArrayToString(UserID)}");
                        UserEntity = new UserEntity(UserID.AsMemory())
                        {
                            Name = Username,
                            DisplayName = UserDisplayName ?? Username,
                        };
                    }
                }

                // Configure credential parameters
                var make = new MakeCredentialParameters(RelyingParty, UserEntity!);
                if (Discoverable)
                {
                    make.AddOption("rk", true);
                }

                // Add HMAC secret extension if supported
                if (fido2Session.AuthenticatorInfo.IsExtensionSupported("hmac-secret"))
                {
                    make.AddHmacSecretExtension(fido2Session.AuthenticatorInfo);
                }

                // Generate client data hash
                var clientData = new
                {
                    type = "webauthn.create",
                    origin = $"https://{RelyingParty.Id}",
                    challenge = Challenge!.Base64URLEncode(),
                };

                var clientDataJSON = System.Text.Json.JsonSerializer.Serialize(clientData);
                var clientDataBytes = System.Text.Encoding.UTF8.GetBytes(clientDataJSON);
                var digester = CryptographyProviders.Sha256Creator();
                _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);
                make.ClientDataHash = digester.Hash!.AsMemory();

                // Add requested algorithms
                WriteDebug("Adding requested Algorithms");
                foreach (var alg in RequestedAlgorithms)
                {
                    WriteDebug($"`tAdding {alg.ToString()}");
                    make.AddAlgorithm("public-key", alg);
                }

                // Create and return the credential
                WriteDebug($"Promting for touch to complete the credential creation...");
                Console.WriteLine("Touch the YubiKey...");
                MakeCredentialData returnvalue = fido2Session.MakeCredential(make);

                var credData = new CredentialData(returnvalue, clientDataJSON, UserEntity!, RelyingParty);
                WriteInformation($"Credential created for {UserEntity!.DisplayName ?? UserEntity.Name} using RP: {RelyingParty.Id}.", new string[] { "FIDO2", "Info" });
                WriteObject(credData);
            }
        }
    }
}
