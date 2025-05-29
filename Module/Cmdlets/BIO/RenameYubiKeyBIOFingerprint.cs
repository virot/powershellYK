/// <summary>
/// Renames a registered biometric fingerprint on a YubiKey.
/// Supports renaming by either fingerprint friendly name or ID.
/// Requires a YubiKey with biometric capabilities.
/// 
/// .EXAMPLE
/// Rename-YubiKeyBIOFingerprint -Name "Right Index" -NewName "Right Thumb"
/// Renames the fingerprint from "Right Index" to "Right Thumb"
/// 
/// .EXAMPLE
/// Rename-YubiKeyBIOFingerprint -ID "1234" -NewName "Left Index"
/// Renames the fingerprint with ID "1234" to "Left Index"
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using System.Security.Cryptography;
using powershellYK.support;
using powershellYK.support.transform;
using Yubico.YubiKey.Fido2.Commands;
using Yubico.YubiKey.Piv;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Rename, "YubiKeyBIOFingerprint")]
    public class RenameYubikeyBIOFingerprintCmdlet : PSCmdlet
    {
        // Parameter for renaming by fingerprint name
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Friendly name of fingerprint to rename", ParameterSetName = "Rename using Name")]
        public String? Name;

        // Parameter for renaming by fingerprint ID
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ID of fingerprint to rename", ParameterSetName = "Rename using ID")]
        [ValidateLength(4, 4)]
        public String? ID;

        // Parameter for the new friendly name
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New friendly name", ParameterSetName = "Rename using ID")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New friendly name", ParameterSetName = "Rename using Name")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public String NewName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                TemplateInfo? fingerprint = null;

                // Find fingerprint based on parameter set
                switch (ParameterSetName)
                {
                    case "Rename using Name":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => x.FriendlyName.ToLower() == Name!.ToLower()).FirstOrDefault();
                        break;
                    case "Rename using ID":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => Converter.ByteArrayToString(x.TemplateId.ToArray()).ToLower() == ID!.ToLower()).FirstOrDefault();
                        break;

                    default:
                        throw new Exception("Invalid ParameterSetName");
                };

                if (fingerprint is not null)
                {
                    // Update the fingerprint's friendly name
                    session.SetBioTemplateFriendlyName(fingerprint.TemplateId, NewName);
                    WriteInformation($"Fingerprint renamed ({(NewName)}).", new string[] { "Biometrics", "Info" });
                }
                else
                {
                    throw new Exception("No fingerprint found.");
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}