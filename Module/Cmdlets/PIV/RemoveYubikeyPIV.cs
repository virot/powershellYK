using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;
using System.Collections.ObjectModel;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubiKeyPIVCmdlet : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to move a key from")]
        public byte Slot { get; set; }
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                if (ShouldProcess($"Key in slot {Slot}", "Remove"))
                {
                    pivSession.DeleteKey(Slot);
                }
            }
        }
    }
}