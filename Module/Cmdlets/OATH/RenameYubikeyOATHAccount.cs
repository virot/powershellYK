using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Rename, "YubikeyOATHAccount")]

    public class RenameYubikeyOATHAccountCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Account to remove")]
        [Alias("Credential")]
        public Credential? Account { get; set; }
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
            };
        }

        protected override void ProcessRecord()
        {
            using (var oathSession = new OathSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                oathSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                oathSession.RenameCredential(Account!, NewIssuer != null ? NewIssuer : Account!.Issuer, NewAccountName != null ? NewAccountName : Account!.AccountName!);
                }
        }
    }
}