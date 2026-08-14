/// <summary>
/// Produces a FIDO2 previewSign signature on a YubiKey (firmware 5.8+).
///
/// This cmdlet hashes the supplied input and signs it on the device. The first
/// run provisions a discoverable previewSign credential, persists the key handle
/// and seed public key under ~/.powershellYK, signs the digest, and returns the
/// material. Later runs with no key referenced reload that store and sign without
/// creating another credential. Pass -NewKey to provision a replacement. A
/// previously returned PreviewSignKey can be piped in, or KeyHandle / PublicKey /
/// CredentialID from a prior Format-List can be supplied explicitly.
///
/// Firmware 5.8 implements previewSign as ARKG-P256/ESP256-split (-65539). The
/// signature is produced on-device through a GetAssertion previewSign request
/// with an ARKG signing ticket. PublicKey is the ARKG seed, not the ESP256
/// verify key.
///
/// .EXAMPLE
/// New-YubiKeyFIDO2Signature -InputData "data"
/// Provisions a previewSign credential (or reuses the stored key) and returns the signature.
///
/// .EXAMPLE
/// $key = New-YubiKeyFIDO2Signature -Path .\document.pdf
/// New-YubiKeyFIDO2Signature -Path .\other.pdf -KeyHandle $key.KeyHandle -PublicKey $key.PublicKey -CredentialID $key.CredentialID
/// Signs a file, then re-signs another file with the key handle copied from the previous output.
///
/// .EXAMPLE
/// $key | New-YubiKeyFIDO2Signature -InputData "data"
/// Re-signs new data by piping a PreviewSignKey. Use -NewKey to force a new credential.
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.New, "YubiKeyFIDO2Signature", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium, DefaultParameterSetName = "Create")]
    public class NewYubikeyFIDO2SignatureCommand : PSCmdlet
    {
        // Default relying party used when provisioning a synthetic previewSign credential
        private const string DefaultRelyingPartyID = "previewsign.powershellyk";

        // Input data as raw bytes / hex string
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Raw data (hex string or byte[]) to hash before signing.")]
        [TransformHexInput]
        public byte[]? InputData { get; set; }

        // Input data from a file
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "File whose contents will be hashed before signing.")]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public FileInfo? Path { get; set; }

        // Digest algorithm
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Digest algorithm used to hash the input prior to signing.")]
        [Alias("Hash")]
        [ValidateSet("SHA256", "SHA384", "SHA512")]
        public string Digest { get; set; } = "SHA256";

        // Re-sign with a previously provisioned previewSign key (carries key handle + credential id)
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Previously provisioned previewSign key to re-sign with.", ParameterSetName = "WithKey")]
        public PreviewSignKey? PreviewSignKey { get; set; }

        // Re-sign with an explicit key handle pasted from a previous output
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Key handle (base64url, hex, or byte[]) from a previous previewSign output.", ParameterSetName = "WithKeyHandle")]
        [TransformHexOrBase64Url]
        public byte[]? KeyHandle { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Seed public key (base64url, hex, or byte[]) from a previous previewSign output.", ParameterSetName = "WithKeyHandle")]
        [TransformHexOrBase64Url]
        public byte[]? PublicKey { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential id the key handle belongs to.", ParameterSetName = "WithKeyHandle")]
        public powershellYK.FIDO2.CredentialID? CredentialID { get; set; }

        // Relying party id (optional; defaults to previewsign.powershellyk)
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Relying party id the key handle belongs to.", ParameterSetName = "WithKeyHandle")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Relying party id for the previewSign credential.", ParameterSetName = "Create")]
        [Alias("RP", "Origin")]
        public string? RelyingPartyID { get; set; }

        // Optional user metadata for the synthetic credential
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Username for the new previewSign credential.", ParameterSetName = "Create")]
        public string? Username { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Display name for the new previewSign credential.", ParameterSetName = "Create")]
        public string? UserDisplayName { get; set; }

        // Require user verification (PIN/biometric) instead of mere user presence
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Require user verification (PIN/biometric) instead of only user presence.", ParameterSetName = "Create")]
        public SwitchParameter UserVerification { get; set; }

        // Force provisioning even when a stored key exists
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Provision a new previewSign credential even when a stored key exists.", ParameterSetName = "Create")]
        public SwitchParameter NewKey { get; set; }

        // Optional output file for the generated key material
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Write the public key, key handle and signature (base64url JSON) to disk.")]
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        public FileInfo? OutFile { get; set; }

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
            // Resolve and validate input source (exactly one of -InputData / -Path)
            if (InputData is not null && Path is not null)
            {
                throw new ArgumentException("Specify either -InputData or -Path, not both.");
            }
            if (InputData is null && Path is null)
            {
                throw new ArgumentException("No input supplied. Provide -InputData or -Path.");
            }

            byte[] inputBytes;
            if (Path is not null)
            {
                string resolvedInputPath = GetUnresolvedProviderPathFromPSPath(Path.FullName);
                try
                {
                    inputBytes = File.ReadAllBytes(resolvedInputPath);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to read input file from '{Path}'.", ex);
                }
            }
            else
            {
                inputBytes = InputData!;
            }

            // Compute the digest that will be signed. ESP256/ARKG-P256 signs a 32-byte digest,
            // so only SHA-256 produces usable input for the previewSign signing path.
            byte[] toBeSigned = ComputeDigest(inputBytes, Digest);
            if (toBeSigned.Length != 32)
            {
                throw new ArgumentException($"Digest '{Digest}' produces a {toBeSigned.Length}-byte hash. The ARKG-P256/ESP256 previewSign signing path requires a 32-byte SHA-256 digest; use -Digest SHA256.");
            }

            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Detect previewSign support
                if (!fido2Session.AuthenticatorInfo.IsExtensionSupported(Extensions.PreviewSign))
                {
                    throw new NotSupportedException("This YubiKey does not support the FIDO2 signing (requires firmware 5.8+).");
                }

                if (ParameterSetName == "Create")
                {
                    EmitCreateOrReuse(fido2Session, toBeSigned);
                }
                else
                {
                    EmitSignedWithExistingKey(fido2Session, toBeSigned);
                }
            }
        }

        // Reuse a stored key when possible; otherwise provision, persist, and sign
        private void EmitCreateOrReuse(Fido2Session fido2Session, byte[] toBeSigned)
        {
            string rpId = string.IsNullOrWhiteSpace(RelyingPartyID) ? DefaultRelyingPartyID : RelyingPartyID!;

            if (!NewKey.IsPresent)
            {
                PreviewSignKey? stored = TryLoadDefaultStore();
                if (stored is not null &&
                    string.Equals(stored.RelyingPartyID, rpId, StringComparison.OrdinalIgnoreCase) &&
                    stored.KeyHandleBytes is not null &&
                    stored.PublicKeyCose is not null &&
                    CredentialExistsOnDevice(fido2Session, stored))
                {
                    WriteDebug($"Reusing stored previewSign key from '{GetDefaultStorePath()}'.");
                    PreviewSignKey? reused = EmitSignedFromMaterial(
                        fido2Session,
                        toBeSigned,
                        stored.CredentialID,
                        rpId,
                        stored.KeyHandleBytes,
                        stored.PublicKeyCose,
                        stored.AlgorithmIdentifier);
                    if (reused is not null)
                    {
                        PersistDefaultStore(reused);
                    }
                    return;
                }
            }

            EmitGeneratedKey(fido2Session, toBeSigned, rpId);
        }

        // Provision a synthetic previewSign credential, sign the digest, and persist the store
        private void EmitGeneratedKey(Fido2Session fido2Session, byte[] toBeSigned, string rpId)
        {
            string target = $"Relying party '{rpId}'";
            string action = "Create a new discoverable FIDO2 previewSign credential and sign the input";
            if (!ShouldProcess(target, action))
            {
                return;
            }

            var relyingParty = new RelyingParty(rpId) { Name = rpId };

            // Stable user id so a later -NewKey replaces the discoverable credential
            byte[] userId = BuildStableUserId(rpId);
            string userName = Username ?? "previewSign";
            var userEntity = new UserEntity(userId.AsMemory())
            {
                Name = userName,
                DisplayName = UserDisplayName ?? userName,
            };

            var make = new MakeCredentialParameters(relyingParty, userEntity);
            make.AddOption("rk", true);

            // Request previewSign key generation for the ARKG-P256/ESP256 algorithm
            var options = UserVerification.IsPresent
                ? PreviewSignOptions.RequireUserVerification
                : PreviewSignOptions.RequireUserPresence;
            make.AddPreviewSignGenerateKeyExtension(
                fido2Session.AuthenticatorInfo,
                new CoseAlgorithmIdentifier[] { powershellYK.support.FIDO2.PreviewSign.ArkgP256ESP256 },
                options);

            // Generate the client data hash (webauthn.create), same approach as New-YubiKeyFIDO2Credential
            make.ClientDataHash = BuildClientDataHash("webauthn.create", rpId);

            WriteDebug("Sending previewSign MakeCredential request to the YubiKey...");
            Console.WriteLine("Touch the YubiKey...");
            MakeCredentialData credentialData = fido2Session.MakeCredential(make);

            PreviewSignGeneratedKey? generatedKey = credentialData.GetPreviewSignGeneratedKey();
            if (generatedKey is null)
            {
                throw new InvalidOperationException("The YubiKey did not return previewSign generated key material.");
            }

            powershellYK.FIDO2.CredentialID credentialID = credentialData.AuthenticatorData.CredentialId!;
            byte[] keyHandle = generatedKey.KeyHandle.ToArray();
            byte[] publicKeyCose = generatedKey.PublicKey.ToArray();

            byte[] signature = SignWithKey(fido2Session, rpId, credentialID, keyHandle, toBeSigned, publicKeyCose);

            var result = new PreviewSignKey(
                credentialID: credentialID,
                relyingPartyID: rpId,
                algorithm: generatedKey.Algorithm,
                hashAlgorithm: Digest,
                toBeSigned: toBeSigned,
                keyHandle: keyHandle,
                publicKeyCose: publicKeyCose,
                signature: signature);

            WriteObject(result);
            WriteOutFile(result);
            PersistDefaultStore(result);
        }

        // Sign the digest using a previously provisioned previewSign key handle
        private void EmitSignedWithExistingKey(Fido2Session fido2Session, byte[] toBeSigned)
        {
            powershellYK.FIDO2.CredentialID credentialID;
            string rpId;
            byte[] keyHandle;
            CoseAlgorithmIdentifier algorithm;
            byte[] seedPublicKeyCose;

            if (ParameterSetName == "WithKey")
            {
                if (PreviewSignKey!.KeyHandleBytes is null)
                {
                    throw new ArgumentException("The supplied PreviewSignKey does not contain a key handle and cannot be used to sign.");
                }
                if (PreviewSignKey.PublicKeyCose is null)
                {
                    throw new ArgumentException("The supplied PreviewSignKey does not contain a seed public key, which is required to derive the ARKG signing ticket.");
                }
                credentialID = PreviewSignKey.CredentialID;
                rpId = string.IsNullOrWhiteSpace(PreviewSignKey.RelyingPartyID) ? DefaultRelyingPartyID : PreviewSignKey.RelyingPartyID!;
                keyHandle = PreviewSignKey.KeyHandleBytes;
                algorithm = PreviewSignKey.AlgorithmIdentifier;
                seedPublicKeyCose = PreviewSignKey.PublicKeyCose;
            }
            else
            {
                if (KeyHandle is null || KeyHandle.Length == 0)
                {
                    throw new ArgumentException("A key handle is required to sign with an existing previewSign key.");
                }
                if (PublicKey is null || PublicKey.Length == 0)
                {
                    throw new ArgumentException("A seed public key is required to derive the ARKG signing ticket.");
                }
                if (!CredentialID.HasValue)
                {
                    throw new ArgumentException("A credential id is required to sign with an existing previewSign key.");
                }
                credentialID = CredentialID.Value;
                rpId = string.IsNullOrWhiteSpace(RelyingPartyID) ? DefaultRelyingPartyID : RelyingPartyID!;
                keyHandle = KeyHandle;
                algorithm = powershellYK.support.FIDO2.PreviewSign.ArkgP256ESP256;
                seedPublicKeyCose = PublicKey;
            }

            EmitSignedFromMaterial(fido2Session, toBeSigned, credentialID, rpId, keyHandle, seedPublicKeyCose, algorithm);
        }

        // Sign with already-resolved key material and emit a reusable PreviewSignKey
        private PreviewSignKey? EmitSignedFromMaterial(
            Fido2Session fido2Session,
            byte[] toBeSigned,
            powershellYK.FIDO2.CredentialID credentialID,
            string rpId,
            byte[] keyHandle,
            byte[] seedPublicKeyCose,
            CoseAlgorithmIdentifier algorithm)
        {
            string target = $"Relying party '{rpId}'";
            string action = "Sign the input with the referenced previewSign key";
            if (!ShouldProcess(target, action))
            {
                return null;
            }

            byte[] signature = SignWithKey(fido2Session, rpId, credentialID, keyHandle, toBeSigned, seedPublicKeyCose);

            var result = new PreviewSignKey(
                credentialID: credentialID,
                relyingPartyID: rpId,
                algorithm: algorithm,
                hashAlgorithm: Digest,
                toBeSigned: toBeSigned,
                keyHandle: keyHandle,
                publicKeyCose: seedPublicKeyCose,
                signature: signature);

            WriteObject(result);
            WriteOutFile(result);
            return result;
        }

        // Perform a GetAssertion previewSign request and return the on-device signature
        private byte[] SignWithKey(Fido2Session fido2Session, string rpId, powershellYK.FIDO2.CredentialID credentialID, byte[] keyHandle, byte[] toBeSigned, ReadOnlyMemory<byte> seedPublicKeyCose)
        {
            var relyingParty = new RelyingParty(rpId);
            byte[] clientDataHash = BuildClientDataHash("webauthn.get", rpId);

            // ARKG-P256 signing requires a derived ticket (additionalArgs). Firmware 5.8
            // returns CTAP "Invalid parameters" if this is omitted.
            var derived = powershellYK.support.FIDO2.PreviewSign.DeriveSigningKey(seedPublicKeyCose, rpId);

            var getParams = new GetAssertionParameters(relyingParty, clientDataHash);
            getParams.AllowCredential(credentialID.ToYubicoFIDO2CredentialID());
            getParams.AddPreviewSignExtension(keyHandle.AsMemory(), toBeSigned.AsMemory(), derived.AdditionalArgs);

            WriteDebug("Requesting assertion with previewSign extension...");
            Console.WriteLine("Touch the YubiKey...");
            IReadOnlyList<GetAssertionData> assertions = fido2Session.GetAssertions(getParams);

            byte[]? signature = assertions[0].AuthenticatorData.GetPreviewSignSignature();
            if (signature is null || signature.Length == 0)
            {
                throw new InvalidOperationException("The YubiKey did not return a previewSign signature.");
            }
            WriteDebug($"Received {signature.Length}-byte previewSign signature from YubiKey.");
            return signature;
        }

        // True when the stored parent credential is still on the authenticator
        private bool CredentialExistsOnDevice(Fido2Session fido2Session, PreviewSignKey stored)
        {
            if (string.IsNullOrWhiteSpace(stored.RelyingPartyID))
            {
                return false;
            }

            IReadOnlyList<RelyingParty> relyingParties;
            try
            {
                relyingParties = fido2Session.EnumerateRelyingParties();
            }
            catch (Exception ex)
            {
                WriteDebug($"Failed to enumerate relying parties while checking the stored previewSign key: {ex.Message}");
                return false;
            }

            RelyingParty? relyingParty = relyingParties.FirstOrDefault(rp =>
                string.Equals(rp.Id, stored.RelyingPartyID, StringComparison.OrdinalIgnoreCase));
            if (relyingParty is null)
            {
                WriteDebug($"Stored relying party '{stored.RelyingPartyID}' is not present on the YubiKey.");
                return false;
            }

            IReadOnlyList<CredentialUserInfo> credentials;
            try
            {
                credentials = fido2Session.EnumerateCredentialsForRelyingParty(relyingParty);
            }
            catch (Exception ex)
            {
                WriteDebug($"Failed to enumerate credentials for '{stored.RelyingPartyID}': {ex.Message}");
                return false;
            }

            byte[] storedId = stored.CredentialID.ToByte();
            bool found = credentials.Any(user => user.CredentialId.Id.ToArray().SequenceEqual(storedId));
            if (!found)
            {
                WriteDebug("Stored previewSign credential id is not present on the YubiKey; a new credential will be created.");
            }
            return found;
        }

        // Load the default per-serial host store, or null when it does not exist
        private PreviewSignKey? TryLoadDefaultStore()
        {
            string storePath = GetDefaultStorePath();
            if (!File.Exists(storePath))
            {
                WriteDebug($"No stored previewSign key at '{storePath}'.");
                return null;
            }

            string json;
            try
            {
                json = File.ReadAllText(storePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read stored previewSign key from '{storePath}'.", ex);
            }

            return PreviewSignKey.FromJson(json);
        }

        // Write the default per-serial host store (creates ~/.powershellYK if needed)
        private void PersistDefaultStore(PreviewSignKey result)
        {
            string storePath = GetDefaultStorePath();
            string? directory = System.IO.Path.GetDirectoryName(storePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                File.WriteAllText(storePath, result.ToJson());
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write stored previewSign key to '{storePath}'.", ex);
            }
            WriteDebug($"Stored previewSign key written to '{storePath}'.");
        }

        // Default store: ~/.powershellYK/previewsign-<serial>.json
        private static string GetDefaultStorePath()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            int? serial = ((YubiKeyDevice)YubiKeyModule._yubikey!).SerialNumber;
            string fileName = serial.HasValue ? $"previewsign-{serial.Value}.json" : "previewsign.json";
            return System.IO.Path.Combine(profile, ".powershellYK", fileName);
        }

        // 16-byte user id derived from the RP id so re-provisioning replaces the same rk credential
        private static byte[] BuildStableUserId(string rpId)
        {
            byte[] hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rpId));
            byte[] userId = new byte[16];
            Buffer.BlockCopy(hash, 0, userId, 0, 16);
            return userId;
        }

        // Build a SHA-256 client data hash for the given WebAuthn ceremony type
        private static byte[] BuildClientDataHash(string type, string rpId)
        {
            byte[] challengeBytes = RandomNumberGenerator.GetBytes(32);
            var clientData = new
            {
                type = type,
                origin = $"https://{rpId}",
                challenge = Convert.ToBase64String(challengeBytes).Replace('+', '-').Replace('/', '_').Replace("=", ""),
            };
            var clientDataJSON = System.Text.Json.JsonSerializer.Serialize(clientData);
            var clientDataBytes = System.Text.Encoding.UTF8.GetBytes(clientDataJSON);
            return SHA256.HashData(clientDataBytes);
        }

        // Optionally persist the key material to disk
        private void WriteOutFile(PreviewSignKey result)
        {
            if (OutFile is null)
            {
                return;
            }

            string resolvedOutputPath = GetUnresolvedProviderPathFromPSPath(OutFile.FullName);
            try
            {
                File.WriteAllText(resolvedOutputPath, result.ToJson());
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write key material to '{resolvedOutputPath}'.", ex);
            }
            WriteInformation(new InformationRecord($"previewSign key material written to {resolvedOutputPath}", "New-YubiKeyFIDO2Signature"));
        }

        // Hash the input with the requested digest algorithm
        private static byte[] ComputeDigest(byte[] data, string digest)
        {
            return digest.ToUpperInvariant() switch
            {
                "SHA256" => SHA256.HashData(data),
                "SHA384" => SHA384.HashData(data),
                "SHA512" => SHA512.HashData(data),
                _ => throw new ArgumentException($"Unsupported digest algorithm '{digest}'."),
            };
        }
    }
}
