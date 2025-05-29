/// <summary>
/// Removes a certificate from a specified YubiKey PIV slot.
/// Requires a YubiKey with PIV support and a valid certificate in the slot.
/// Note: This functionality is not yet implemented.
/// 
/// .EXAMPLE
/// Remove-YubiKeyPIVCertificate -Slot "PIV Authentication"
/// Removes the certificate from the PIV Authentication slot
/// 
/// .EXAMPLE
/// Remove-YubiKeyPIVCertificate -Slot "Digital Signature"
/// Removes the certificate from the Digital Signature slot
/// </summary>

// Imports
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
        // Parameter for the PIV slot
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to remove a key from")]
        public PIVSlot Slot { get; set; }

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Get current certificate from slot
                X509Certificate2? currentCertificate;
                try
                {
                    currentCertificate = pivSession.GetCertificate(Slot);
                }
                catch
                {
                    throw new Exception($"No certificate found in PIV slot {Slot}.");
                }

                // Remove certificate if user confirms
                if (ShouldProcess($"Certificate in slot {Slot}, subjectname: '{currentCertificate.SubjectName}'", "Remove"))
                {
                    throw new NotImplementedException("Remove-YubiKeyPIVCertificate not implemented.");
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}