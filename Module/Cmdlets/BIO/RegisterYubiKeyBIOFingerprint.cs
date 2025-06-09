/// <summary>
/// Registers a new biometric fingerprint on a YubiKey.
/// Guides the user through the fingerprint enrollment process.
/// Requires a YubiKey with biometric capabilities.
/// 
/// .EXAMPLE
/// Register-YubiKeyBIOFingerprint -Name "Right Index"
/// Registers a new fingerprint with the name "Right Index"
/// 
/// .EXAMPLE
/// Register-YubiKeyBIOFingerprint
/// Registers a new fingerprint without a specific name
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
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Register, "YubiKeyBIOFingerprint")]
    public class RegisterYubikeyBIOFingerprintCmdlet : Cmdlet
    {
        // Parameter for the fingerprint name
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Name of the finger to register")]
        public String? Name;

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

                try
                {
                    // Guide user through fingerprint enrollment process
                    WriteInformation("Place your finger against the sensor repeatedly...", new string[] { "Biometrics", "prompt" });

                    TemplateInfo fingerprint = session.EnrollFingerprint(Name, null);

                    WriteInformation($"Fingerprint registered ({Name ?? "Unnamed"}).", new string[] { "Biometrics", "Info" });

                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to register fingerprint ({Name ?? "Unnamed"}): {ex.Message}", ex);
                }
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}