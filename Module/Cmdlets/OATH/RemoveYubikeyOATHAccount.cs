/// <summary>
/// Removes an OATH credential from the YubiKey OATH applet.
/// Requires a YubiKey with OTP support.
/// If no YubiKey is selected, automatically calls Connect-Yubikey first.
/// Note: This operation cannot be undone.
/// 
/// .EXAMPLE
/// Get-YubiKeyOATHAccount | Where-Object { $_.Issuer -eq "Example" } | Remove-YubiKeyOATHAccount
/// Removes all OATH credentials from a specific issuer
/// 
/// .EXAMPLE
/// $cred = Get-YubiKeyOATHAccount | Select-Object -First 1
/// Remove-YubiKeyOATHAccount -Account $cred
/// Removes a specific OATH credential
/// </summary>

// Imports
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Remove, "YubiKeyOATHAccount", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyOATHAccountCommand : Cmdlet
    {
        // Parameters for credential identification
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential to remove")]
        [Alias("Credential")]
        public Credential Account { get; set; } = new Credential();

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
                oathSession.RemoveCredential(Account);
            }
        }
    }
}
