using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using powershellYK.PIV;
using System.Security.Cryptography.X509Certificates;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Remove, "YubiKeyPIVCertificate", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubiKeyPIVCertificateCmdlet : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to remove a key from")]
        public PIVSlot Slot { get; set; }
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                X509Certificate2? currentCertificate;

                try
                {
                    currentCertificate = pivSession.GetCertificate(Slot);
                }
                catch
                {
                    throw new Exception($"No certificate found in PIV slot {Slot}.");
                }

                if (ShouldProcess($"Certificate in slot {Slot}, subjectname: '{currentCertificate.SubjectName}'", "Remove"))
                {
                    throw new NotImplementedException("Remove-YubiKeyPIVCertificate not implemented.");
                    
                }
            }
        }

    }
}