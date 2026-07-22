/// <summary>
/// Allows uploading of large blobs to the YubiKey FIDO2 applet, associated with a specific credential ID or relying party.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
///
/// .EXAMPLE
/// Import-YubiKeyFIDO2Blob -LargeBlob test.txt -RelyingPartyID "demo.yubico.com"
/// Imports a file as a large blob when there is no more than one credential for the Relying Party on the YubiKey.
///
/// .EXAMPLE
/// Import-YubiKeyFIDO2Blob -LargeBlob test.txt -CredentialId "19448fe...67ab9207071e"
/// Imports a file as a large blob for a specified FIDO2 credential by ID (use when the RP has multiple credentials).
///
/// .EXAMPLE
/// Import-YubiKeyFIDO2Blob -LargeBlob test.txt -CredentialId "19448fe...67ab9207071e" -Force
/// Imports a file as a large blob and overwrites any existing blob entry for that credential without prompting.
/// </summary>

using Microsoft.VisualBasic;
using Newtonsoft.Json;
using powershellYK.FIDO2;
using powershellYK.support;
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Security;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Cryptography;
using Yubico.YubiKey.Fido2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Clear, "YubiKeyFIDO2Blob", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ClearYubikeyFIDO2BlobCmdlet : PSCmdlet
    {
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

                if (fido2Session.AuthenticatorInfo.MaximumSerializedLargeBlobArray is null)
                {
                    throw new NotSupportedException("This YubiKey does not support FIDO2 large blobs.");
                }

                // Get the current serialized Large Blob array from the authenticator
                var blobArray = fido2Session.GetSerializedLargeBlobArray();
                WriteDebug($"Step 6: Current large blob array loaded! {blobArray.Entries.Count} entries");

                for (int i = 0; i < blobArray.Entries.Count; i++)
                {
                    blobArray.RemoveEntry(i);
                }

                if (ShouldProcess("This will delete stored FIDO2 blobs, Proceed?", "This will delete stored FIDO2 blobs, Proceed?", "WARNING!"))
                {
                    WriteDebug($"Step 8: Writing updated large blob array ({blobArray.Encode().Length}) bytes to YubiKey...");
                    // Write the updated Large Blob array back to the authenticator
                    fido2Session.SetSerializedLargeBlobArray(blobArray);

                    WriteInformation(
                        $"All FIDO2 large blob entries removed.",
                        new[] { "FIDO2" });
                }
            }
        }
    }
}
