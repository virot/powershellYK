/// <summary>
/// Moves a key pair from one YubiKey PIV slot to another.
/// Optionally migrates the associated certificate to the new slot.
/// Requires a YubiKey with PIV support and key movement capability.
/// 
/// .EXAMPLE
/// Move-YubiKeyPIV -SourceSlot "PIV Authentication" -DestinationSlot "Digital Signature"
/// Moves the key pair from PIV Authentication to Digital Signature slot
/// 
/// .EXAMPLE
/// Move-YubiKeyPIV -SourceSlot "Key Management" -DestinationSlot "Card Authentication" -MigrateCertificate
/// Moves the key pair and its certificate to the new slot
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using System.Security.Cryptography.X509Certificates;
using powershellYK.PIV;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Move, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class MoveYubiKeyPIVCmdlet : Cmdlet
    {
        // Parameter for source slot
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to move a key from")]
        public PIVSlot SourceSlot { get; set; }

        // Parameter for destination slot
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to move a key to")]
        public PIVSlot DestinationSlot { get; set; }

        // Parameter for certificate migration
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Move the certificate along")]
        public SwitchParameter MigrateCertificate { get; set; }

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Validate source and destination slots
            if (SourceSlot == DestinationSlot)
            {
                throw new ArgumentException("Source and destination slot cannot be the same.");
            }

            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Check if YubiKey supports key movement
                if (!((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivMoveOrDeleteKey))
                {
                    throw new Exception("YubiKey version does not support moving keys.");
                }

                // Get certificate if migration is requested
                X509Certificate2? slotCert = null;
                if (MigrateCertificate.IsPresent)
                {
                    try
                    {
                        WriteDebug("MigrateCertificate is requested, getting certificate from source slot...");
                        slotCert = pivSession.GetCertificate(SourceSlot);
                    }
                    catch { }
                }

                // Move the key pair
                pivSession.MoveKey(SourceSlot, DestinationSlot);

                if (MigrateCertificate.IsPresent)
                {
                    if (slotCert is X509Certificate2)
                    {
                        WriteDebug("MigrateCertificate is requested, trying to import certificate...");
                        pivSession.ImportCertificate(DestinationSlot, slotCert);
                    }
                }
            }
        }
    }
}