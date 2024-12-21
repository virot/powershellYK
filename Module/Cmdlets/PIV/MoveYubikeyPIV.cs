using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using System.Security.Cryptography.X509Certificates;
using powershellYK.PIV;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Move, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class MoveYubiKeyPIVCmdlet : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to move a key from")]
        public PIVSlot SourceSlot { get; set; }
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to move a key to")]
        public PIVSlot DestinationSlot { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Move the certificate along")]
        public SwitchParameter MigrateCertificate { get; set; }
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }
        protected override void ProcessRecord()
        {
            if (SourceSlot == DestinationSlot)
            {
                throw new ArgumentException("Source and destination slot cannot be the same");
            }
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (! ((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivMoveOrDeleteKey))
                {
                    throw new Exception("YubiKey version does not support moving keys");
                }

                X509Certificate2? slotCert = null;

                if (MigrateCertificate.IsPresent)
                {
                    try
                    {
                        WriteDebug("MigrateCertificate is requested, getting certificate from source slot");
                        slotCert = pivSession.GetCertificate(SourceSlot);
                    }
                    catch { }
                }
                pivSession.MoveKey(SourceSlot, DestinationSlot);

                if (MigrateCertificate.IsPresent)
                {
                    if (slotCert is X509Certificate2)
                    {
                        WriteDebug("MigrateCertificate is requested, trying to import certificate");
                        pivSession.ImportCertificate(DestinationSlot, slotCert);
                    }
                }
            }
        }
    }
}