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
    [Cmdlet(VerbsCommon.Get, "YubikeyUVFingerprint")]
    public class GetYubikeyUVFingerprintCmdlet : Cmdlet
    {

        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        protected override void ProcessRecord()
        {
            using (var session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (session.GetBioModality() == BioModality.Fingerprint)
                {
                    List<Fingerprint> fingerprints = new List<Fingerprint>();
                    // powershellYK.UserValidation
                    //IReadOnlyList<TemplateInfo> bio = 
                    foreach(var enrollment in session.EnumerateBioEnrollments().ToArray())
                    {
                        fingerprints.Add(new Fingerprint(enrollment.TemplateId, enrollment.FriendlyName));
                    }
                    WriteObject(fingerprints);
                }
                else
                {
                    WriteWarning("YubiKey does not support reading fingerprints.");
                }
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
