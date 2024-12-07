using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyOATHAccount", ConfirmImpact = ConfirmImpact.High)]

    public class RemoveYubikeyOATHAccountCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential to remove")]
        [Alias("Credential")]
        public Credential Account { get; set; } = new Credential();
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                oathSession.RemoveCredential(Account);
            }
        }
    }
}
