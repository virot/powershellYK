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
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Friendly name of fingerprint to rename", ParameterSetName = "Rename using Name")]
        public String? Name;
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ID of fingerprint to rename", ParameterSetName = "Rename using ID")]
        [ValidateLength(4, 4)]
        public String? ID;
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New friendly name", ParameterSetName = "Rename using ID")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "New friendly name", ParameterSetName = "Rename using Name")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public String NewName;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
                    case "Rename using Name":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => x.FriendlyName.ToLower() == Name!.ToLower()).FirstOrDefault();
                        break;
                    case "Rename using ID":
                        fingerprint = session.EnumerateBioEnrollments().Where(x => HexConverter.ByteArrayToString(x.TemplateId.ToArray()).ToLower() == ID!.ToLower()).FirstOrDefault();
                        break;

                    default:
                        throw new Exception("Invalid ParameterSetName");
                };

                if (fingerprint is not null)
                {
                    session.SetBioTemplateFriendlyName(fingerprint.TemplateId, NewName);
                    WriteInformation($"Fingerprint renamed ({(NewName)}).", new string[] { "Biometrics", "Info" });
                }
                else
                {
                    throw new Exception("No fingerprint found.");
                }
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}