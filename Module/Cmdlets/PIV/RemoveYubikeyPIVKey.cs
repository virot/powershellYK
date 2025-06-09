/// <summary>
/// Removes a key pair from a specified YubiKey PIV slot.
/// Requires a YubiKey with PIV support and a firmware that supports key deletion.
/// 
/// .EXAMPLE
/// Remove-YubiKeyPIVKey -Slot "PIV Authentication"
/// Removes the key pair from the PIV Authentication slot
/// 
/// .EXAMPLE
/// Remove-YubiKeyPIVKey -Slot "Digital Signature"
/// Removes the key pair from the Digital Signature slot
/// </summary>

// Imports
using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using powershellYK.PIV;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Remove, "YubiKeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubiKeyPIVKeyCmdlet : Cmdlet
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

                // Check if YubiKey supports key deletion
                if (!((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivMoveOrDeleteKey))
                {
                    throw new Exception("YubiKey version does not support removing keys.");
                }

                // Remove key if user confirms
                if (ShouldProcess($"Key in slot {Slot}", "Remove"))
                {
                    try
                    {
                        // Verify key exists before attempting deletion
                        var _ = pivSession.GetMetadata(Slot).PublicKeyParameters;
                        pivSession.DeleteKey(Slot);
                        WriteInformation($"Removed key(s) from PIV slot {Slot}.", new string[] { "PIV", "Info" });
                    }
                    catch
                    {
                        WriteWarning($"No key(s) found in PIV slot {Slot}. Nothing to remove.");
                    }
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}