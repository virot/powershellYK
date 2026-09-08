/// <summary>
/// Encrypts a file using FIDO2 PRF (hmac-secret) extension on a YubiKey.
/// Uses HKDF-SHA256 for key derivation and AES-256-GCM for authenticated encryption.
/// Requires a YubiKey with FIDO2 hmac-secret support, a FIDO2 PIN, and administrator
/// privileges on Windows.
/// When no credential or relying party is provided, the cmdlet automatically creates a
/// synthetic FIDO2 credential (RP and username both set to "prf-encryption") and
/// uses it for encryption. On YubiKey firmware 5.8+ (hmac-secret-mc), first-time
/// AutoCreate derives the PRF during credential creation so only one touch is required.
/// Older firmware enables hmac-secret at creation then requests the PRF with a second
/// assertion. Reuse of an existing credential always uses a single GetAssertions call.
///
/// .EXAMPLE
/// Protect-YubiKeyFIDO2File -Path .\secret.txt
/// Encrypts secret.txt, automatically creating or reusing a "prf-encryption" credential.
///
/// .EXAMPLE
/// Protect-YubiKeyFIDO2File -Path .\secret.txt -Confirm:$false
/// Same as above but skips the credential-creation confirmation prompt.
///
/// .EXAMPLE
/// $cred = Get-YubiKeyFIDO2Credential | Where-Object { $_.RelyingParty.Id -eq "demo.yubico.com" }
/// Protect-YubiKeyFIDO2File -Path .\secret.txt -Credential $cred
/// Encrypts secret.txt using the specified FIDO2 credential.
///
/// .EXAMPLE
/// Get-Item .\secret.txt | Protect-YubiKeyFIDO2File -Credential $cred
/// Encrypts a file via pipeline input.
///
/// .EXAMPLE
/// Protect-YubiKeyFIDO2File -Path .\secret.txt -RelyingPartyID "demo.yubico.com"
/// Encrypts using the sole credential for that relying party (aliases -RP and -Origin).
///
/// .EXAMPLE
/// Get-YubiKeyFIDO2Credential -RelyingPartyID "demo.yubico.com"
/// Protect-YubiKeyFIDO2File -Path .\secret.txt -CredentialId "19448fe...67ab9207071e" -RelyingPartyID "demo.yubico.com"
/// When the RP has several credentials, list them with Get-YubiKeyFIDO2Credential, then pass the chosen -CredentialId (hex or base64url) together with -RelyingPartyID.
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using powershellYK.support.FIDO2;
using powershellYK.support.transform;
using powershellYK.support.validators;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsSecurity.Protect, "YubiKeyFIDO2File", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "AutoCreate")]
    public class ProtectYubiKeyFIDO2FileCmdlet : PSCmdlet
    {
        // Parameters for file input/output
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Input file to encrypt.")]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public required FileInfo Path { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Output file path. Defaults to input path with .enc extension.")]
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        public FileInfo? OutFile { get; set; }

        // Parameters for credential identification
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Credential object from Get-YubiKeyFIDO2Credential.", ParameterSetName = "WithCredential")]
        public Credential? Credential { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Credential ID to use.", ParameterSetName = "WithCredentialID")]
        public CredentialID CredentialID { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "WithCredentialID",
            HelpMessage = "Relying Party ID for the assertion.")]
        [Parameter(
            Mandatory = true,
            ParameterSetName = "ByRelyingPartyID",
            HelpMessage = "Relying party ID or display name when that RP has a single credential on the YubiKey.")]
        [Alias("RP", "Origin")]
        [ValidateNotNullOrEmpty]
        public string? RelyingPartyID { get; set; }

        private const string AutoCreateRpId = "prf-encryption";
        private const string AutoCreateUsername = "prf-encryption";

        // HKDF domain separation info string for key derivation
        private static readonly byte[] HkdfInfo = "powershellYK/fido2prf/v1"u8.ToArray();

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
            string resolvedInputPath = GetUnresolvedProviderPathFromPSPath(Path.FullName);
            string resolvedOutputPath = OutFile is not null
                ? GetUnresolvedProviderPathFromPSPath(OutFile.FullName)
                : resolvedInputPath + ".enc";

            FileInfo displayOutput = OutFile ?? new FileInfo(Path.FullName + ".enc");
            string encryptConfirm =
                $"This will encrypt '{Path.FullName}' using the FIDO Pseudo-Random Function (PRF) extension (to: '{displayOutput.FullName}'). Proceed?";
            if (!ShouldProcess(encryptConfirm, encryptConfirm, "WARNING!"))
                return;

            // Read input file and generate per-file random salt (provider-aware path, same as Import-YubiKeyFIDO2Blob)
            byte[] plaintext;
            try
            {
                plaintext = File.ReadAllBytes(resolvedInputPath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read plaintext from file '{Path}'.", ex);
            }

            byte[] salt = RandomNumberGenerator.GetBytes(PRFEncryptedFile.SaltLength);

            byte[] credIdBytes;
            string rpId;
            byte[]? prfOutput = null;

            if (ParameterSetName == "AutoCreate")
            {
                rpId = AutoCreateRpId;
                credIdBytes = null!;

                using (var lookupSession = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    lookupSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    var rps = lookupSession.EnumerateRelyingParties();
                    var match = rps.FirstOrDefault(rp =>
                        string.Equals(rp.Id, rpId, StringComparison.OrdinalIgnoreCase));
                    if (match is not null)
                    {
                        var creds = lookupSession.EnumerateCredentialsForRelyingParty(match);
                        if (creds.Count > 0)
                        {
                            credIdBytes = creds[0].CredentialId.Id.ToArray();
                            WriteDebug($"AutoCreate: reusing existing credential {Convert.ToHexString(credIdBytes).ToLowerInvariant()} for RP '{rpId}'.");
                        }
                    }
                }

                if (credIdBytes is null)
                {
                    string username = AutoCreateUsername;
                    if (!ShouldProcess(
                        $"No credential or relying party was provided. A new FIDO2 credential will be created on RP '{rpId}'. Continue?",
                        "Create FIDO2 credential"))
                    {
                        return;
                    }

                    using (var createSession = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                    {
                        createSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                        if (createSession.AuthenticatorInfo.IsExtensionSupported(Extensions.HmacSecretMc))
                        {
                            WriteDebug("AutoCreate: hmac-secret-mc supported (firmware 5.8+); creating credential and deriving PRF in one operation...");
                            Console.WriteLine("Touch the YubiKey...");
                            var mcResult = Fido2PrfHelper.TryCreateCredentialWithHmacSecretMc(createSession, rpId, username, salt);
                            if (mcResult is not null)
                            {
                                credIdBytes = mcResult.Value.CredentialId;
                                prfOutput = mcResult.Value.PrfOutput;
                                WriteDebug($"AutoCreate: hmac-secret-mc credential created, ID {Convert.ToHexString(credIdBytes).ToLowerInvariant()}, {prfOutput.Length}-byte PRF.");
                            }
                        }
                        else
                        {
                            WriteDebug("AutoCreate: hmac-secret-mc not supported on this firmware.");
                        }
                    }

                    if (prfOutput is null)
                    {
                        WriteDebug($"AutoCreate: falling back to New-YubiKeyFIDO2Credential + assertion for RP '{rpId}', user '{username}'...");
                        var ps = PowerShell.Create(RunspaceMode.CurrentRunspace)
                            .AddCommand("New-YubiKeyFIDO2Credential")
                            .AddParameter("RelyingPartyID", rpId)
                            .AddParameter("Username", username)
                            .AddParameter("Confirm", false);
                        if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                            ps.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);

                        var results = ps.Invoke();
                        if (results.Count == 0 || results[0].BaseObject is not CredentialData credData)
                            throw new InvalidOperationException("Failed to create a FIDO2 credential for file encryption.");

                        credIdBytes = credData.CredentialId!.Value.ToArray();
                        WriteDebug($"AutoCreate: credential created, ID {Convert.ToHexString(credIdBytes).ToLowerInvariant()}");
                    }
                }
            }
            else if (ParameterSetName == "WithCredential")
            {
                credIdBytes = Credential!.CredentialID.ToByte();
                rpId = Credential.RelyingParty.Id!;
            }
            else if (ParameterSetName == "ByRelyingPartyID")
            {
                if (string.IsNullOrWhiteSpace(RelyingPartyID))
                {
                    throw new ArgumentNullException(nameof(RelyingPartyID), "A relying party ID or name must be provided.");
                }

                using (var resolveSession = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    resolveSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    var relyingParties = resolveSession.EnumerateRelyingParties();
                    var matchingRps = relyingParties.Where(rpMatch =>
                            string.Equals(rpMatch.Id, RelyingPartyID, StringComparison.OrdinalIgnoreCase) ||
                            (!string.IsNullOrWhiteSpace(rpMatch.Name) && string.Equals(rpMatch.Name, RelyingPartyID, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    if (matchingRps.Count == 0)
                    {
                        throw new ArgumentException($"No relying party found matching '{RelyingPartyID}' on this YubiKey.", nameof(RelyingPartyID));
                    }

                    if (matchingRps.Count > 1)
                    {
                        string rpCandidates = string.Join(", ", matchingRps.Select(rpMatch => $"'{rpMatch.Id}'"));
                        throw new InvalidOperationException(
                            $"Multiple relying parties matched '{RelyingPartyID}': {rpCandidates}. " +
                            "Use a specific RP ID with -RelyingPartyID, or specify -CredentialId with -RelyingPartyID.");
                    }

                    RelyingParty credentialRelyingParty = matchingRps[0];
                    try
                    {
                        var credentialsForOrigin = resolveSession.EnumerateCredentialsForRelyingParty(credentialRelyingParty);
                        if (credentialsForOrigin.Count == 0)
                        {
                            throw new InvalidOperationException($"No credentials found for relying party '{credentialRelyingParty.Id}'.");
                        }

                        if (credentialsForOrigin.Count > 1)
                        {
                            string candidateCredentialIds = string.Join(", ",
                                credentialsForOrigin.Select(c => Convert.ToHexString(c.CredentialId.Id.ToArray()).ToLowerInvariant()));
                            throw new InvalidOperationException(
                                $"Relying party '{credentialRelyingParty.Id}' has multiple credentials ({credentialsForOrigin.Count}). " +
                                $"Use Get-YubiKeyFIDO2Credential -RelyingPartyID {credentialRelyingParty.Id} to list credentials, then use -Credential or -CredentialId.");
                        }

                        credIdBytes = credentialsForOrigin[0].CredentialId.Id.ToArray();
                        rpId = credentialRelyingParty.Id!;
                    }
                    catch (NotSupportedException)
                    {
                        throw new InvalidOperationException(
                            $"Unable to enumerate credentials for relying party '{credentialRelyingParty.Id}' due to unsupported algorithm.");
                    }
                }
            }
            else
            {
                credIdBytes = CredentialID.ToByte();
                rpId = RelyingPartyID!;
            }

            if (credIdBytes is null)
            {
                throw new InvalidOperationException("Failed to resolve a FIDO2 credential for file encryption.");
            }

            // hmac-secret-mc already returned the PRF during MakeCredential; otherwise request it via GetAssertions
            if (prfOutput is null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    WriteDebug("Requesting assertion with hmac-secret extension...");
                    Console.WriteLine("Touch the YubiKey...");
                    prfOutput = Fido2PrfHelper.GetPrfViaAssertion(fido2Session, rpId, credIdBytes, salt);
                }
            }

            WriteDebug($"Received {prfOutput.Length}-byte PRF output from YubiKey.");

            // Derive AES-256 key using HKDF-SHA256 with domain separation
            byte[] derivedKey = HKDF.DeriveKey(
                HashAlgorithmName.SHA256,
                ikm: prfOutput,
                outputLength: 32,
                salt: null,
                info: HkdfInfo);

            // Encrypt plaintext with AES-256-GCM
            byte[] iv = RandomNumberGenerator.GetBytes(PRFEncryptedFile.IVLength);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[PRFEncryptedFile.TagLength];

            using (var aes = new AesGcm(derivedKey, tagSizeInBytes: PRFEncryptedFile.TagLength))
            {
                aes.Encrypt(iv, plaintext, ciphertext, tag);
            }

            // Write encrypted file with structured header
            var encryptedFile = new PRFEncryptedFile(credIdBytes, rpId, salt, iv, tag, ciphertext);
            try
            {
                File.WriteAllBytes(resolvedOutputPath, encryptedFile.Serialize());
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write encrypted file to '{resolvedOutputPath}'.", ex);
            }

            WriteInformation(new InformationRecord($"Encrypted file written to {resolvedOutputPath}", "Protect-YubiKeyFIDO2File"));
            WriteObject(new FileInfo(resolvedOutputPath));
        }
    }
}
