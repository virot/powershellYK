/// <summary>
/// Allows clearing all FIDO2 large blob entries from a YubiKey.
/// Requires a YubiKey with FIDO2 support and administrator privileges on Windows.
///
/// .EXAMPLE
/// Clear-YubiKeyFIDO2Blob
/// Removes all FIDO2 large blob entries from the connected YubiKey.
/// </summary>

using powershellYK.support;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
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

                for (int i = (blobArray.Entries.Count - 1); i >= 0; i--)
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