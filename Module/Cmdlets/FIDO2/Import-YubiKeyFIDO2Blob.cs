/// <summary>
/// Allows uploading of large blobs to the YubiKey FIDO2 applet, associated with a specific credential ID or relying party.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// Sends minimum PIN length to specified relying party
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -LargeBlob test.txt -RelyingPartyID "demo.yubico.com"
/// Imports a file as a large blob when there is no more than one credential for the Relying Party on the YubiKey
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -LargeBlob test.txt -CredentialId "19448fe...67ab9207071e"
/// Imports a file as a large blob for a specified FIDO2 Credential by ID (handles multiple entries for the same Relying Party)
/// 
/// .EXAMPLE
/// cd C:\CODE
/// Set-YubiKeyFIDO2 -LargeBlob test.txt -CredentialId "19448fe...67ab9207071e" -Force
/// Imports a file as a large blob and overwrites any existing blob entry for that credential without prompting
/// </summary>

using Microsoft.VisualBasic;
using Newtonsoft.Json;
using powershellYK.FIDO2;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.Collections.ObjectModel;
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Security;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Cryptography;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsData.Import, "YubiKeyFIDO2Blob")]
    public class ImportYubikeyFIDO2BlobCmdlet : PSCmdlet
    {
        // Parameters for large blob import
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Set LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "File to import as large blob"
        )]
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Set LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "File to import as large blob"
        )]
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public required System.IO.FileInfo LargeBlob { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Set LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "Credential ID (hex or base64url string) to associate with the large blob array."
        )]
        public powershellYK.FIDO2.CredentialID? CredentialId { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Set LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "Relying party ID, or relying party display name if unique, to associate with the large blob."
        )]
        [Alias("RP", "Origin")]
        [ValidateNotNullOrEmpty]
        public string? RelyingPartyID { get; set; }

        [Parameter(
            Mandatory = false,
            ParameterSetName = "Set LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "Overwrite existing large blob entry for this credential without prompting."
        )]
        [Parameter(
            Mandatory = false,
            ParameterSetName = "Set LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "Overwrite existing large blob entry for this credential without prompting."
        )]
        public SwitchParameter Force { get; set; }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Check if running as Administrator
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }

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
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                switch (ParameterSetName)
                {
                    case "Set LargeBlob":
                    case "Set LargeBlob by RelyingPartyID":
                        // Verify the YubiKey supports large blobs
                        if (fido2Session.AuthenticatorInfo.MaximumSerializedLargeBlobArray is null)
                        {
                            throw new NotSupportedException("This YubiKey does not support FIDO2 large blobs.");
                        }
                        WriteDebug($"Step 1: Large blob support verified (max {fido2Session.AuthenticatorInfo.MaximumSerializedLargeBlobArray.Value} bytes).");

                        if (LargeBlob is null)
                        {
                            throw new ArgumentException("You must enter a valid file path.", nameof(LargeBlob));
                        }

                        // Resolve and read the input file
                        string resolvedPath = GetUnresolvedProviderPathFromPSPath(LargeBlob.FullName);
                        byte[] blobData;
                        try
                        {
                            blobData = System.IO.File.ReadAllBytes(resolvedPath);
                            WriteDebug($"Step 2: Input file loaded from '{LargeBlob.FullName}' ({blobData.Length} bytes).");
                        }
                        catch (Exception ex)
                        {
                            throw new IOException($"Failed to read large blob data from file '{LargeBlob}'.", ex);
                        }

                        // Resolve target credential and corresponding relying party.
                        RelyingParty? credentialRelyingParty = null;
                        var relyingParties = fido2Session.EnumerateRelyingParties();
                        powershellYK.FIDO2.CredentialID selectedCredentialId;
                        if (ParameterSetName == "Set LargeBlob by RelyingPartyID")
                        {
                            if (string.IsNullOrWhiteSpace(RelyingPartyID))
                            {
                                throw new ArgumentNullException(nameof(RelyingPartyID), "A relying party ID/name must be provided when setting a large blob by RelyingPartyID.");
                            }

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
                                    "Use a specific RP ID with -RelyingPartyID, or specify -CredentialId directly.");
                            }

                            credentialRelyingParty = matchingRps[0];
                            try
                            {
                                var credentialsForOrigin = fido2Session.EnumerateCredentialsForRelyingParty(credentialRelyingParty);
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
                                        $"Use Get-YubiKeyFIDO2Credential -RelyingPartyID {credentialRelyingParty.Id} to list credentials, then use -CredentialId to choose which credential to use.");
                                }

                                selectedCredentialId = (powershellYK.FIDO2.CredentialID)credentialsForOrigin[0].CredentialId;
                            }
                            catch (NotSupportedException)
                            {
                                throw new InvalidOperationException(
                                    $"Unable to enumerate credentials for relying party '{credentialRelyingParty.Id}' due to unsupported algorithm.");
                            }
                        }
                        else
                        {
                            // Ensure a credential ID was supplied
                            if (CredentialId is null)
                            {
                                throw new ArgumentNullException(nameof(CredentialId), "A FIDO2 credential ID must be provided when setting a large blob.");
                            }

                            selectedCredentialId = CredentialId.Value;
                            byte[] credentialIdBytes = selectedCredentialId.ToByte();

                            foreach (RelyingParty currentRp in relyingParties)
                            {
                                try
                                {
                                    var credentials = fido2Session.EnumerateCredentialsForRelyingParty(currentRp);
                                    foreach (var credInfo in credentials)
                                    {
                                        if (credInfo.CredentialId.Id.ToArray().SequenceEqual(credentialIdBytes))
                                        {
                                            credentialRelyingParty = currentRp;
                                            break;
                                        }
                                    }
                                    if (credentialRelyingParty is not null)
                                    {
                                        break;
                                    }
                                }
                                catch (NotSupportedException)
                                {
                                    // Skip relying parties with unsupported algorithms
                                    continue;
                                }
                            }

                            if (credentialRelyingParty is null)
                            {
                                throw new ArgumentException($"Credential with ID '{selectedCredentialId}' not found on this YubiKey.", nameof(CredentialId));
                            }
                        }
                        WriteDebug($"Step 3: Target resolved to RP '{credentialRelyingParty.Id}' and credential '{selectedCredentialId}'.");

                        // Create client data hash for GetAssertion
                        byte[] challengeBytes = new byte[32];
                        RandomNumberGenerator.Fill(challengeBytes);
                        var clientData = new
                        {
                            type = "webauthn.get",
                            origin = $"https://{credentialRelyingParty.Id}",
                            challenge = Convert.ToBase64String(challengeBytes)
                        };
                        var clientDataJSON = JsonConvert.SerializeObject(clientData);
                        var clientDataBytes = System.Text.Encoding.UTF8.GetBytes(clientDataJSON);
                        var digester = CryptographyProviders.Sha256Creator();
                        _ = digester.TransformFinalBlock(clientDataBytes, 0, clientDataBytes.Length);
                        ReadOnlyMemory<byte> clientDataHash = digester.Hash!.AsMemory();
                        WriteDebug($"Step 4: Client data hash created for origin '{clientData.origin}'.");

                        // Perform GetAssertion to retrieve the largeBlobKey
                        var gaParams = new GetAssertionParameters(credentialRelyingParty, clientDataHash);

                        // Add the credential ID to the allow list (for non-resident keys)
                        gaParams.AllowCredential(selectedCredentialId.ToYubicoFIDO2CredentialID());

                        // Request the largeBlobKey extension
                        gaParams.AddExtension(Extensions.LargeBlobKey, new byte[] { 0xF5 });

                        // Execute assertion ceremony
                        Console.WriteLine("Touch the YubiKey...");
                        var assertions = fido2Session.GetAssertions(gaParams);
                        if (assertions.Count == 0)
                        {
                            throw new InvalidOperationException("GetAssertion returned no assertions.");
                        }

                        // Retrieve the per-credential largeBlobKey
                        var retrievedKey = assertions[0].LargeBlobKey;
                        if (retrievedKey is null)
                        {
                            throw new NotSupportedException("The credential does not support large blob keys. The credential may need to be recreated with the largeBlobKey extension.");
                        }
                        WriteDebug($"Step 5: Assertion completed and largeBlobKey retrieved ({assertions.Count} assertion(s)).");

                        // Get the current serialized Large Blob array from the authenticator
                        var blobArray = fido2Session.GetSerializedLargeBlobArray();
                        WriteDebug($"Step 6: Current large blob array loaded ({blobArray.Entries.Count} entries).");

                        // Enforce one entry per credential key by detecting existing decryptable entries.
                        var matchingEntryIndexes = new List<int>();
                        for (int i = 0; i < blobArray.Entries.Count; i++)
                        {
                            if (blobArray.Entries[i].TryDecrypt(retrievedKey.Value, out _))
                            {
                                matchingEntryIndexes.Add(i);
                            }
                        }

                        if (matchingEntryIndexes.Count > 0)
                        {
                            string existingMsg =
                                $"Found {matchingEntryIndexes.Count} existing large blob entr{(matchingEntryIndexes.Count == 1 ? "y" : "ies")} " +
                                $"for relying party '{credentialRelyingParty.Id}'.";
                            WriteWarning(existingMsg);

                            bool overwriteExisting = Force.IsPresent;
                            if (!overwriteExisting)
                            {
                                overwriteExisting = ShouldContinue(
                                    $"{existingMsg} Overwrite existing entr{(matchingEntryIndexes.Count == 1 ? "y" : "ies")}?",
                                    "Large blob entry already exists");
                            }

                            if (!overwriteExisting)
                            {
                                WriteWarning("Operation cancelled by user. Existing large blob entries were left unchanged.");
                                return;
                            }

                            for (int i = matchingEntryIndexes.Count - 1; i >= 0; i--)
                            {
                                blobArray.RemoveEntry(matchingEntryIndexes[i]);
                            }
                        }

                        WriteDebug($"Step 7: Adding blob entry ({blobData.Length} bytes).");
                        // Add a new encrypted entry, binding the data to the retrieved largeBlobKey
                        blobArray.AddEntry(blobData, retrievedKey.Value);

                        WriteDebug("Step 8: Writing updated large blob array to YubiKey...");
                        // Write the updated Large Blob array back to the authenticator
                        fido2Session.SetSerializedLargeBlobArray(blobArray);

                        WriteInformation(
                            $"FIDO2 large blob entry added successfully for Relying Party (Origin): '{credentialRelyingParty.Id}'.",
                            new[] { "FIDO2", "LargeBlob" });
                        break;
                }
            }
        }
    }
}
