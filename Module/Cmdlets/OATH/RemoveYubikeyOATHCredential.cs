using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Otp;

namespace powershellYK.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyOATHCredential", ConfirmImpact = ConfirmImpact.High)]

    public class RemoveYubikeyOATHCredentialCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Credential to remove")]
        public Credential? Credential { get; set; }
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._oathSession is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyOath");
                myPowersShellInstance.Invoke();
            }
        }
        protected override void ProcessRecord()
        {
            if (Credential is not null && ShouldProcess($"{Credential.Name}", "Remove"))
            {
                YubiKeyModule._oathSession!.RemoveCredential(Credential);
            }
        }
    }
}