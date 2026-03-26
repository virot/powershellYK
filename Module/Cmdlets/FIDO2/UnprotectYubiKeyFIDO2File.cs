/// <summary>
/// Decrypts a file previously encrypted with Protect-YubiKeyFIDO2File.
/// Reads the credential ID and relying party ID from the encrypted file header,
/// then uses the YubiKey's FIDO2 hmac-secret extension to derive the decryption key.
///
/// .EXAMPLE
/// Unprotect-YubiKeyFIDO2File -Path .\secret.txt.enc
/// Decrypts the file, writing output to secret.txt
///
/// .EXAMPLE
/// Unprotect-YubiKeyFIDO2File -Path .\secret.txt.enc -OutFile .\recovered.txt
/// Decrypts the file to a specific output path
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
    [Cmdlet(VerbsSecurity.Unprotect, "YubiKeyFIDO2File", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class UnProtectYubiKeyFIDO2FileCmdlet : PSCmdlet
    {
        // Parameters for file input/output
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Encrypted file to decrypt.")]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public required FileInfo Path { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Output file path. Defaults to stripping .enc extension.")]
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        public FileInfo? OutFile { get; set; }

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
            string resolvedDefaultOutput = resolvedInputPath.EndsWith(".enc", StringComparison.OrdinalIgnoreCase)
                ? resolvedInputPath[..^4]
                : resolvedInputPath + ".dec";
            string resolvedOutputPath = OutFile is not null
                ? GetUnresolvedProviderPathFromPSPath(OutFile.FullName)
                : resolvedDefaultOutput;

            FileInfo displayOutput = OutFile ?? new FileInfo(
                Path.FullName.EndsWith(".enc", StringComparison.OrdinalIgnoreCase)
                    ? Path.FullName[..^4]
                    : Path.FullName + ".dec");

            // Read and parse the encrypted file (provider-aware path, same as Import-YubiKeyFIDO2Blob / Protect-YubiKeyFIDO2File)
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(resolvedInputPath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to read encrypted file from '{Path}'.", ex);
            }

            PRFEncryptedFile encryptedFile;
            try
            {
                encryptedFile = PRFEncryptedFile.Deserialize(fileData);
            }
            catch (InvalidDataException ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, "InvalidEncryptedFile", ErrorCategory.InvalidData, Path));
                return;
            }

            string decryptConfirm =
                $"This will decrypt '{Path.FullName}' using the FIDO Pseudo-Random Function (PRF) extension (to: '{displayOutput.FullName}'). Proceed?";
            if (!ShouldProcess(decryptConfirm, decryptConfirm, "WARNING!"))
                return;

            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN operations
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Build client data hash for the assertion using header metadata
                var relyingParty = new RelyingParty(encryptedFile.RelyingPartyId);
                var clientData = new { type = "webauthn.get", origin = $"https://{encryptedFile.RelyingPartyId}", challenge = Convert.ToBase64String(encryptedFile.Salt) };
                var clientDataJSON = System.Text.Json.JsonSerializer.Serialize(clientData);
                var clientDataBytes = System.Text.Encoding.UTF8.GetBytes(clientDataJSON);
                var digester = CryptographyProviders.Sha256Creator();
                _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);

                // Request assertion with hmac-secret extension to get PRF output
                var getParams = new GetAssertionParameters(relyingParty, digester.Hash!);
                getParams.RequestHmacSecretExtension(encryptedFile.Salt);
                getParams.AllowCredential(new CredentialID(encryptedFile.CredentialId).ToYubicoFIDO2CredentialID());

                WriteDebug("Requesting assertion with hmac-secret extension for decryption...");
                Console.WriteLine("Touch the YubiKey...");
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

                // Decrypt ciphertext with AES-256-GCM
                byte[] plaintext = new byte[encryptedFile.Ciphertext.Length];

                using (var aes = new AesGcm(derivedKey, tagSizeInBytes: PRFEncryptedFile.TagLength))
                {
                    aes.Decrypt(encryptedFile.IV, encryptedFile.Ciphertext, encryptedFile.Tag, plaintext);
                }

                // Write decrypted output
                try
                {
                    File.WriteAllBytes(resolvedOutputPath, plaintext);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to write decrypted file to '{resolvedOutputPath}'.", ex);
                }

                WriteInformation(new InformationRecord($"Decrypted file written to {resolvedOutputPath}", "Unprotect-YubiKeyFIDO2File"));
                WriteObject(new FileInfo(resolvedOutputPath));
            }
        }
    }
}
