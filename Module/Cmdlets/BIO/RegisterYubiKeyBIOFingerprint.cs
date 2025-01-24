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
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Name of the finger to register")]
        public String? Name;

        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        protected override void ProcessRecord()
        {
            using (var session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                try
                {
                    // TODO: There should ideally be a prompt for each read.
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