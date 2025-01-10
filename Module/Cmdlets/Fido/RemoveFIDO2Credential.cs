using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using System.Linq;
using powershellYK.support;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Remove, "YubikeyFIDO2Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveYubikeyFIDO2CredentialCmdlet : PSCmdlet
    {
        // Credential ID is required when calling the cmdlet.
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential ID to remove")]
        [ValidateNotNullOrEmpty]
        public required byte[] CredentialId { get; set; }

        protected override void BeginProcessing()
        {
            // If no FIDO2 PIN exists, we need to connect to the FIDO2 application
            if (YubiKeyModule._fido2PIN is null)
            {
                WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction")) //TODO: @virot not sure why MyInvocation protests...
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
                if (YubiKeyModule._fido2PIN is null)
                {
                    throw new Exception("Connect-YubikeyFIDO2 failed to connect to the FIDO2 applet!");
                }
            }


            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        protected override void ProcessRecord()
    {
            if (ShouldProcess($"Credential ID {(CredentialId)}", "Remove"))
            {
                using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                    // Check if the requested CredentialID exist on the YubiKey


                    /*
                     * TODO: @virot help please ;) 
                    var credentials = fido2Session.EnumerateCredentialsForRelyingParty
                        
                    if (!credentials.Any(c => c.CredentialId.SequenceEqual(CredentialId)))
                    {
                        throw new Exception("Credential not found on the YubiKey.");
                    }
                    */

                    // Delete the credential
                    fido2Session.DeleteCredential(CredentialId);
                    WriteInformation("Credential removed.", new string[] { "FIDO2", "Info" });
                }
            }
        }
    }
}
