﻿using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.support.validators;
using powershellYK.PIV;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Remove, "YubiKeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubiKeyPIVKeyCmdlet : Cmdlet
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

                if (!((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivMoveOrDeleteKey))
                {
                    throw new Exception("YubiKey version does not support removing keys.");
                }

                if (ShouldProcess($"Key in slot {Slot}", "Remove"))
                {
                    try
                    {
                        //  This will throw an exception if no key is found in the slot
                        var _ = pivSession.GetMetadata(Slot).PublicKey;
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

    }
}