/// <summary>
/// Renames an OATH credential in the YubiKey OATH applet.
/// Allows changing both the issuer and account name.
/// Requires a YubiKey with OTP support.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// 
/// .EXAMPLE
/// $cred = Get-YubiKeyOATHAccount | Select-Object -First 1
/// Rename-YubiKeyOATHAccount -Account $cred -NewAccountName "newuser@example.com"
/// Renames the account name of a specific OATH credential
/// 
/// .EXAMPLE
/// $cred = Get-YubiKeyOATHAccount | Select-Object -First 1
/// Rename-YubiKeyOATHAccount -Account $cred -NewIssuer "NewCompany"
/// Renames the issuer of a specific OATH credential
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Rename, "YubiKeyOATHAccount")]
    public class RenameYubikeyOATHAccountCommand : Cmdlet
    {
        // Parameters for credential identification
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Account to rename")]
        [Alias("Credential")]
        public Credential? Account { get; set; }

        // Parameters for new credential information
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New Account name")]
        public string? NewAccountName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "New Issuer")]
        public string? NewIssuer { get; set; }

        // Initialize processing and verify requirements
        protected override void BeginProcessing()
        {
            // Connect to YubiKey if not already connected
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                myPowersShellInstance.Invoke();
                WriteDebug($"Successfully connected.");
            }
        }

        // Process the main cmdlet logic
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
