/// <summary>
/// Retrieves biometric fingerprint information from a YubiKey.
/// Lists all enrolled fingerprints with their template IDs and friendly names.
/// Requires a YubiKey with biometric capabilities.
/// 
/// .EXAMPLE
/// Get-YubiKeyBIOFingerprint
/// Lists all enrolled fingerprints on the connected YubiKey
/// 
/// .EXAMPLE
/// $fingerprints = Get-YubiKeyBIOFingerprint
/// $fingerprints | ForEach-Object { Write-Host "Fingerprint: $($_.FriendlyName)" }
/// Retrieves fingerprints and displays their friendly names
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
using powershellYK.UserValidation;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyBIOFingerprint")]
    public class GetYubikeyBIOFingerprintCmdlet : Cmdlet
    {
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

                if (session.GetBioModality() == BioModality.Fingerprint)
                {
                    List<Fingerprint> fingerprints = new List<Fingerprint>();

                    // Iterate through all enrolled fingerprints
                    foreach (var enrollment in session.EnumerateBioEnrollments().ToArray())
                    {
                        fingerprints.Add(new Fingerprint(enrollment.TemplateId, enrollment.FriendlyName));
                    }
                    WriteObject(fingerprints);
                }
                else
                {
                    WriteWarning("This YubiKey model does not support biometrics!");
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
