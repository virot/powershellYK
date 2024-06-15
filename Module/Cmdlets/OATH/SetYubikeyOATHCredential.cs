using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Set, "YubikeyOATHCredential")]

    public class SetYubikeyOATHCredentialCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential to remove")]
        public Credential? Credential { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New AccountName")]
        public string? NewAccountName { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New Issuer")]
        public string? NewIssuer { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected");
            }
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                oathSession.RenameCredential(Credential!, NewIssuer != null ? NewIssuer : Credential!.Issuer, NewAccountName != null ? NewAccountName : Credential!.AccountName!);
                }
        }
    }
}