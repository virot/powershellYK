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
    [Cmdlet(VerbsCommon.Remove, "YubikeyUVFingerprint", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyUVFingerprintCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Name of finger to remove", ParameterSetName = "Remove using Name")]
        public String? Name;
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ID of finger to remove", ParameterSetName = "Remove using ID")]
        [ValidateLength(4, 4)]
        public String? ID;

        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        protected override void ProcessRecord()
        {
            using (var session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                TemplateInfo? fingerprint = null;

                switch (ParameterSetName)
                {
                    case "Remove using Name":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => x.FriendlyName.ToLower() == Name!.ToLower()).FirstOrDefault();
                        break;
                    case "Remove using ID":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => HexConverter.ByteArrayToString(x.TemplateId.ToArray()).ToLower() == ID!.ToLower()).FirstOrDefault();
                        break;

                    default:
                        throw new Exception("Invalid ParameterSetName");
                };

                if (fingerprint is not null)
                {
                    if (ShouldProcess("This will remove the selected fingerprint from the YubiKey", "This will remove the fingerprint from the YubiKey", "Remove Fingerprint?"))
                    {
                        session.TryRemoveBioTemplate(fingerprint.TemplateId);
                    }
                }
                else
                {
                    throw new Exception("No fingerprint found");
                }
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
