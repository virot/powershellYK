/// <summary>
/// Encrypts a file using FIDO2 PRF (hmac-secret) extension on a YubiKey.
/// Uses HKDF-SHA256 for key derivation and AES-256-GCM for authenticated encryption.
/// Requires a YubiKey with FIDO2 hmac-secret support and administrator privileges on Windows.
///
/// .EXAMPLE
/// $cred = Get-YubiKeyFIDO2Credential | Where-Object { $_.RelyingParty.Id -eq "myapp.local" }
/// Protect-YubiKeyFIDO2 -Path .\secret.txt -Credential $cred
/// Encrypts secret.txt using the specified FIDO2 credential
///
/// .EXAMPLE
/// Get-Item .\secret.txt | Protect-YubiKeyFIDO2 -Credential $cred
/// Encrypts a file via pipeline input
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Cryptography;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsSecurity.Protect, "YubiKeyFIDO2", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "WithCredential")]
    public class ProtectYubiKeyFIDO2Cmdlet : PSCmdlet
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

        [Parameter(Mandatory = true, HelpMessage = "Relying Party ID for the assertion.", ParameterSetName = "WithCredentialID")]
        public required string RelyingPartyID { get; set; }

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
            // Resolve credential ID and relying party from parameter set
            byte[] credIdBytes;
            string rpId;

            if (ParameterSetName == "WithCredential")
            {
                credIdBytes = Credential!.CredentialID.ToByte();
                rpId = Credential.RelyingParty.Id!;
            }
            else
            {
                credIdBytes = CredentialID.ToByte();
                rpId = RelyingPartyID;
            }

            FileInfo outputFile = OutFile ?? new FileInfo(Path.FullName + ".enc");

            if (!ShouldProcess(Path.FullName, "Encrypt file with FIDO2 PRF"))
                return;

            // Read input file and generate per-file random salt
            byte[] plaintext = File.ReadAllBytes(Path.FullName);
            byte[] salt = RandomNumberGenerator.GetBytes(PRFEncryptedFile.SaltLength);

            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Build client data hash for the assertion
                var relyingParty = new RelyingParty(rpId);
                var clientData = new { type = "webauthn.get", origin = $"https://{rpId}", challenge = Convert.ToBase64String(salt) };
                var clientDataJSON = System.Text.Json.JsonSerializer.Serialize(clientData);
                var clientDataBytes = System.Text.Encoding.UTF8.GetBytes(clientDataJSON);
                var digester = CryptographyProviders.Sha256Creator();
                _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);

                // Request assertion with hmac-secret extension to get PRF output
                var getParams = new GetAssertionParameters(relyingParty, digester.Hash!);
                getParams.RequestHmacSecretExtension(salt);
                getParams.AllowCredential(new CredentialID(credIdBytes).ToYubicoFIDO2CredentialID());

                WriteDebug("Requesting assertion with hmac-secret extension...");
                IReadOnlyList<GetAssertionData> assertions = fido2Session.GetAssertions(getParams);

                byte[] prfOutput = assertions[0].AuthenticatorData.GetHmacSecretExtension(fido2Session.AuthProtocol);
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
                File.WriteAllBytes(outputFile.FullName, encryptedFile.Serialize());

                WriteInformation(new InformationRecord($"Encrypted file written to {outputFile.FullName}", "Protect-YubiKeyFIDO2"));
                WriteObject(new FileInfo(outputFile.FullName));
            }
        }
    }
}
