using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Commands;
using powershellYK.UserValidation;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Register, "YubikeyUVFingerprint")]
    public class RegisterYubikeyUVFingerprintCmdlet : Cmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Name of finger to register")]
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
                TemplateInfo fingerprint = session.EnrollFingerprint(Name,null);
                WriteObject(new Fingerprint(fingerprint.TemplateId, fingerprint.FriendlyName));

            }
        }

        protected override void EndProcessing()
        {
        }
    }
}