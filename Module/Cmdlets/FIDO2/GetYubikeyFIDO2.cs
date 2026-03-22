/// <summary>
/// Retrieves information about the FIDO2 applet on a YubiKey.
/// Returns details about supported features, capabilities, and current settings.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2
/// Returns information about the FIDO2 applet on the connected YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2 | Format-List
/// Returns detailed FIDO2 information in a list format
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2 -LargeBlob -OutFile fileName.txt -RelyingPartyID "demo.yubico.com"
/// Exports a large blob to file when there is no more than one credential for the Relying Party on the YubiKey
/// 
/// .EXAMPLE
/// Get-YubiKeyFIDO2 -LargeBlob -OutFile fileName.txt -CredentialId "19448fe...67ab9207071e"
/// Exports a large blob to file for a specified FIDO2 Credential by ID (handles multiple entries for the same Relying Party)
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using Yubico.YubiKey.Cryptography;
using System.Security.Cryptography;
using Newtonsoft.Json;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyFIDO2", DefaultParameterSetName = "GetInfo")]
    public class GetYubikeyFIDO2Cmdlet : PSCmdlet
    {
        // Parameters for large blob export
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "Export large blob for the specified credential"
        )]
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "Export large blob for the specified relying party"
        )]
        public SwitchParameter LargeBlob { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "Credential ID (hex or base64url string) to export large blob for."
        )]
        public powershellYK.FIDO2.CredentialID? CredentialId { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "Relying Party ID (Origin), or relying party display name if unique, to export large blob for."
        )]
        [Alias("RP", "Origin")]
        [ValidateNotNullOrEmpty]
        public string? RelyingPartyID { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob",
            ValueFromPipeline = false,
            HelpMessage = "Output file path for the exported large blob"
        )]
        [Parameter(
            Mandatory = true,
            ParameterSetName = "Export LargeBlob by RelyingPartyID",
            ValueFromPipeline = false,
            HelpMessage = "Output file path for the exported large blob"
        )]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        public System.IO.FileInfo? OutFile { get; set; }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }

            // Connect to FIDO2 if exporting large blob
            if (ParameterSetName == "Export LargeBlob" || ParameterSetName == "Export LargeBlob by RelyingPartyID")
            {
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
                if (ParameterSetName == "Export LargeBlob" || ParameterSetName == "Export LargeBlob by RelyingPartyID")
                {
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    // Verify the YubiKey supports large blobs
                    if (fido2Session.AuthenticatorInfo.MaximumSerializedLargeBlobArray is null)
                    {
                        throw new NotSupportedException("This YubiKey does not support FIDO2 large blobs.");
                    }
                    WriteDebug($"Step 1: Large blob support verified (max {fido2Session.AuthenticatorInfo.MaximumSerializedLargeBlobArray.Value} bytes).");

                    if (OutFile is null)
                    {
                        throw new ArgumentException("You must enter a valid output file path.", nameof(OutFile));
                    }

                    // Resolve target credential and corresponding relying party.
                    RelyingParty? credentialRelyingParty = null;
                    var relyingParties = fido2Session.EnumerateRelyingParties();
                    powershellYK.FIDO2.CredentialID selectedCredentialId;
                    if (ParameterSetName == "Export LargeBlob by RelyingPartyID")
                    {
                        if (string.IsNullOrWhiteSpace(RelyingPartyID))
                        {
                            throw new ArgumentNullException(nameof(RelyingPartyID), "A relying party ID/name must be provided when exporting a large blob by RelyingPartyID.");
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
                            var credentialsForRp = fido2Session.EnumerateCredentialsForRelyingParty(credentialRelyingParty);
                            if (credentialsForRp.Count == 0)
                            {
                                throw new InvalidOperationException($"No credentials found for relying party '{credentialRelyingParty.Id}'.");
                            }
                            if (credentialsForRp.Count > 1)
                            {
                                string candidateCredentialIds = string.Join(", ",
                                    credentialsForRp.Select(c => Convert.ToHexString(c.CredentialId.Id.ToArray()).ToLowerInvariant()));
                                throw new InvalidOperationException(
                                    $"Relying party '{credentialRelyingParty.Id}' has multiple credentials ({credentialsForRp.Count}). " +
                                    $"Use Get-YubiKeyFIDO2Credential -RelyingPartyID {credentialRelyingParty.Id} to list credentials, then use -CredentialId to choose which credential to export.");
                            }

                            selectedCredentialId = (powershellYK.FIDO2.CredentialID)credentialsForRp[0].CredentialId;
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
                            throw new ArgumentNullException(nameof(CredentialId), "A FIDO2 credential ID must be provided when exporting a large blob.");
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
                    WriteDebug($"Step 2: Target resolved to RP '{credentialRelyingParty.Id}' and credential '{selectedCredentialId}'.");

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
                    WriteDebug($"Step 3: Client data hash created for origin '{clientData.origin}'.");

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
                    WriteDebug($"Step 4: Assertion completed and largeBlobKey retrieved ({assertions.Count} assertion(s)).");

                    // Get the current serialized Large Blob array from the authenticator
                    var blobArray = fido2Session.GetSerializedLargeBlobArray();
                    WriteDebug($"Step 5: Current large blob array loaded ({blobArray.Entries.Count} entries).");

                    byte[]? blobData = null;
                    int matchingEntryCount = 0;
                    int selectedEntryIndex = -1;

                    // Iterate entries and decrypt with this credential's largeBlobKey.
                    // If multiple entries match, pick the newest (highest index).
                    for (int i = 0; i < blobArray.Entries.Count; i++)
                    {
                        if (blobArray.Entries[i].TryDecrypt(retrievedKey.Value, out Memory<byte> decrypted))
                        {
                            matchingEntryCount++;
                            blobData = decrypted.ToArray();
                            selectedEntryIndex = i;
                        }
                    }

                    if (matchingEntryCount == 0 || blobData is null)
                    {
                        throw new InvalidOperationException($"No large blob entry found for credential '{selectedCredentialId}'.");
                    }
                    if (matchingEntryCount > 1)
                    {
                        WriteWarning(
                            $"Found {matchingEntryCount} large blob entries for credential '{selectedCredentialId}'. " +
                            $"Using newest entry at index {selectedEntryIndex}. " +
                            "Use Set-YubiKeyFIDO2 -LargeBlob and choose overwrite to compact to a single entry.");
                    }
                    WriteDebug($"Step 6: Blob entry selected from index {selectedEntryIndex} ({blobData.Length} bytes).");

                    WriteDebug($"Step 7: Writing blob data to '{OutFile.FullName}'.");
                    // Write the blob data to the output file
                    string resolvedPath = GetUnresolvedProviderPathFromPSPath(OutFile.FullName);
                    try
                    {
                        System.IO.File.WriteAllBytes(resolvedPath, blobData);
                    }
                    catch (Exception ex)
                    {
                        throw new IOException($"Failed to write large blob data to file '{OutFile}'.", ex);
                    }

                    WriteInformation(
                        $"FIDO2 large blob exported successfully for Relying Party (Origin): '{credentialRelyingParty.Id}'.",
                        new[] { "FIDO2", "LargeBlob" });
                }
                else
                {
                    // Get and output FIDO2 authenticator information
                    AuthenticatorInfo info = fido2Session.AuthenticatorInfo;
                    WriteObject(new Information(info));
                }
            }
        }
    }
}
