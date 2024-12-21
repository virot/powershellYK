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
    [Cmdlet(VerbsLifecycle.Register, "YubikeyUVFingerprint")]
    public class RegisterYubikeyUVFingerprintCmdlet : Cmdlet
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

                TemplateInfo fingerprint = session.EnrollFingerprint(Name, null);

                WriteObject(fingerprint);
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}