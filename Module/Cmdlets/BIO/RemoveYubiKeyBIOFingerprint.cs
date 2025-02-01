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
    [Cmdlet(VerbsCommon.Remove, "YubiKeyBIOFingerprint", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyBIOFingerprintCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Name of fingerprint to remove", ParameterSetName = "Remove using Name")]
        public String? Name;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "ID of fingerprint to remove", ParameterSetName = "Remove using ID")]
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
                        fingerprint = session.EnumerateBioEnrollments()
                            .FirstOrDefault(x => x.FriendlyName.Equals(Name, StringComparison.OrdinalIgnoreCase));
                        break;

                    case "Remove using ID":
                        fingerprint = session.EnumerateBioEnrollments()
                            .FirstOrDefault(x => Converter.ByteArrayToString(x.TemplateId.ToArray())
                                .Equals(ID, StringComparison.OrdinalIgnoreCase));
                        break;

                    default:
                        throw new Exception("Invalid ParameterSetName");
                }

                if (fingerprint is not null)
                {
                    if (ShouldProcess($"This will remove the fingerprint '{(Name ?? ID)}' from the YubiKey", "Remove Fingerprint?"))
                    {
                        bool removed = session.TryRemoveBioTemplate(fingerprint.TemplateId);
                        if (removed)
                        {
                            WriteInformation($"Fingerprint '{(Name ?? ID)}' successfully deleted.", new string[] { "Biometrics", "Info" });
                        }
                        else
                        {
                            throw new Exception("Failed to remove the fingerprint from the YubiKey.");
                        }
                    }
                }
                else
                {
                    throw new Exception($"No fingerprint found with {(Name is not null ? $"name '{Name}'" : $"ID '{ID}'")}.");
                }
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
