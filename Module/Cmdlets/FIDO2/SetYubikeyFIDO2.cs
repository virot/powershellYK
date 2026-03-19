/// <summary>
/// Configures FIDO2 settings on a YubiKey.
/// Supports setting/changing PIN, minimum PIN length, and force PIN change.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
/// Note: Setting PIN is deprecated, use Set-YubiKeyFIDO2PIN instead.
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -SetPIN -NewPIN (ConvertTo-SecureString "123456" -AsPlainText -Force)
/// Sets a new FIDO2 PIN (deprecated, use Set-YubiKeyFIDO2PIN instead)
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -MinimumPINLength 8
/// Sets the minimum PIN length to 8 characters
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -ForcePINChange
/// Forces PIN change on next use
/// 
/// .EXAMPLE
/// Set-YubiKeyFIDO2 -MinimumPINRelyingParty "example.com"
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

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;
using System.Security;
using powershellYK.support.validators;
using System.Collections.ObjectModel;
using Yubico.YubiKey.Piv;
using Microsoft.VisualBasic;
using Yubico.YubiKey.Cryptography;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Set, "YubiKeyFIDO2")]
    public class SetYubikeyFIDO2Cmdlet : PSCmdlet, IDynamicParameters
    {
        // Parameters for PIN configuration
        [Parameter(Mandatory = false, ParameterSetName = "Set PIN", ValueFromPipeline = false, HelpMessage = "Easy access to Set new PIN")]
        public SwitchParameter SetPIN;

        [ValidateRange(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Set PIN minimum length", ValueFromPipeline = false, HelpMessage = "Set the minimum length of the PIN")]
        public int? MinimumPINLength { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Set force PIN change", HelpMessage = "Enable the **_forceChangePin__** flag as supported by YubiKeys with firmware `5.7` or later.\nWhen set, the feature will force the user to change the FIDO2 applet PIN on first use.")]
        public SwitchParameter ForcePINChange { get; set; }

        [ValidateLength(4, 63)]
        [Parameter(Mandatory = true, ParameterSetName = "Send MinimumPIN to RelyingParty", ValueFromPipeline = false, HelpMessage = "To which RelyingParty should minimum PIN be sent")]
        public string? MinimumPINRelyingParty { get; set; }

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
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        public System.IO.FileInfo? LargeBlob { get; set; }

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

        // Get dynamic parameters based on YubiKey state
        public object GetDynamicParameters()
        {
            Collection<Attribute> oldPIN, newPIN;
            if (YubiKeyModule._yubikey is not null)
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Set minimum PIN length based on YubiKey capabilities
                    int minPinLength = fido2Session.AuthenticatorInfo.MinimumPinLength ?? 4;
                    // Verify that the yubikey FIDO2 has a PIN already set. If there is a PIN set then make sure we get the old PIN.
                    if (fido2Session.AuthenticatorInfo.Options!.Any(x => x.Key == AuthenticatorOptions.clientPin && x.Value == true) && (YubiKeyModule._fido2PIN is null))
                    {
                        oldPIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = true, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }
                    else
                    {
                        oldPIN = new Collection<Attribute>() {
                            new ParameterAttribute() { Mandatory = false, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                            new ValidateYubikeyPIN(4, 63)
                        };
                    }

                    // Configure new PIN parameter with minimum length
                    newPIN = new Collection<Attribute>() {
                        new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO2 module.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                        new ValidateYubikeyPIN(minPinLength, 63)
                    };
                }
            }
            else
            {
                // Default parameters when no YubiKey is connected
                oldPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = false, HelpMessage = "Old PIN, required to change the PIN code.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
                newPIN = new Collection<Attribute>() {
                    new ParameterAttribute() { Mandatory = true, HelpMessage = "New PIN code to set for the FIDO2 module.", ParameterSetName = "Set PIN", ValueFromPipeline = false},
                    new ValidateYubikeyPIN(4, 63)
                };
            }

            // Create and return dynamic parameters
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var runtimeDefinedOldPIN = new RuntimeDefinedParameter("OldPIN", typeof(SecureString), oldPIN);
            var runtimeDefinedNewPIN = new RuntimeDefinedParameter("NewPIN", typeof(SecureString), newPIN);
            runtimeDefinedParameterDictionary.Add("OldPIN", runtimeDefinedOldPIN);
            runtimeDefinedParameterDictionary.Add("NewPIN", runtimeDefinedNewPIN);
            return runtimeDefinedParameterDictionary;
        }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to appropriate interface based on operation
            if (ParameterSetName == "Set PIN")
            {
                // Connect to YubiKey if not already connected
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No Yubikey selected, calling Connect-Yubikey...");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
            }
            else
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
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                switch (ParameterSetName)
                {
                    case "Set PIN minimum length":
                        // Verify support for minimum PIN length
                        if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.setMinPINLength) == OptionValue.True)
                        {
                            // Set minimum PIN length
                            if (!fido2Session.TrySetPinConfig(MinimumPINLength, null, null))
                            {
                                throw new Exception("Failed to change the minimum PIN length.");
                            }
                            // Force PIN change
                            fido2Session.TrySetPinConfig(null, null, null);
                            WriteInformation("Minimum PIN length set.", new string[] { "FIDO2", "Info" });
                        }
                        else
                        {
                            throw new Exception("Changing minimum PIN is not supported in this YubiKey firmware version.");
                        }
                        break;

                    // Set force PIN change will expire the PIN on initial use forcing the user to set a new PIN.
                    case "Set force PIN change":
                        // Verify support for force PIN change
                        if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.setMinPINLength) == OptionValue.True)
                        {
                            // Enable force PIN change
                            bool? forceChangePin = true;
                            if (fido2Session.TrySetPinConfig(null, null, forceChangePin))
                            {
                                WriteInformation("Force PIN Change set.", new string[] { "FIDO2", "Info" });
                            }
                            else
                            {
                                // Throw an exception if applying the setting fails.
                                throw new InvalidOperationException("Failed to enforce PIN change.");
                            }
                        }
                        else
                        {
                            // Throw an exception if the hardware does not support the feature.
                            throw new NotSupportedException("Forcing PIN change is not supported in this YubiKey firmware version.");
                        }
                        break;

                    case "Set PIN":
                        // Handle PIN setting/changing
                        if (this.MyInvocation.BoundParameters.ContainsKey("OldPIN"))
                        {
                            YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["OldPIN"];
                        }

                        YubiKeyModule._fido2PINNew = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
                        try
                        {
                            if (fido2Session.AuthenticatorInfo.GetOptionValue(AuthenticatorOptions.clientPin) == OptionValue.False)
                            {
                                WriteDebug("No FIDO PIN set, setting new PIN...");
                                fido2Session.SetPin();
                            }
                            else
                            {
                                WriteDebug("FIDO2 PIN set, changing PIN...");
                                fido2Session.ChangePin();
                            }
                        }
                        catch (Exception e)
                        {
                            YubiKeyModule._fido2PIN = null;
                            throw new Exception(e.Message, e);
                        }
                        finally
                        {
                            YubiKeyModule._fido2PINNew = null;
                        }
                        YubiKeyModule._fido2PIN = (SecureString)this.MyInvocation.BoundParameters["NewPIN"];
                        WriteWarning("Changing FIDO2 PIN using Set-YubiKeyFIDO2 has been deprecated, use Set-YubiKeyFIDO2PIN instead.");
                        WriteInformation("FIDO PIN updated.", new string[] { "FIDO2", "Info" });
                        break;

                    case "Send MinimumPIN to RelyingParty":
                        // Send minimum PIN length to relying party
                        var rpidList = new List<string>(1);
                        RelyingParty rp = new RelyingParty(MinimumPINRelyingParty!);
                        rpidList.Add(rp.Id);
                        if (!fido2Session.TrySetPinConfig(null, rpidList, null))
                        {
                            throw new Exception("Failed to set RelyingParty that will be sent Minimum PIN length.");
                        }
                        break;

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
