using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;


namespace powershellYK.Cmdlets.Fido
{
    [Alias("Get-YubiKeyFIDO2Credentials")]
    [Cmdlet(VerbsCommon.Get, "YubiKeyFIDO2Credential", DefaultParameterSetName = "List-All")]

    public class GetYubikeyFIDO2CredentialsCommand : PSCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "List all", ParameterSetName = "List-All", DontShow = true)]
        public SwitchParameter All { get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential ID to remove", ParameterSetName = "List-CredentialID")]
        public powershellYK.FIDO2.CredentialID? CredentialID { get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Credential ID to remove int Base64 URL encoded format", ParameterSetName = "List-CredentialID-Base64URL")]
        public string? CredentialIdBase64Url { get; set; } = string.Empty;

        protected override void BeginProcessing()
        {
            // If no FIDO2 PIN exists, we need to connect to the FIDO2 application
            if (YubiKeyModule._fido2PIN is null)
            {
                WriteDebug("No FIDO2 session has been authenticated, calling Connect-YubikeyFIDO2...");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFIDO2");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
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
            if (!this.CredentialID.HasValue && CredentialIdBase64Url is not null)
            {
                this.CredentialID = powershellYK.FIDO2.CredentialID.FromStringBase64URL(CredentialIdBase64Url);
            }
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                var relyingParties = fido2Session.EnumerateRelyingParties();

                if (!relyingParties.Any()) // Check if there are no relying parties
                {
                    WriteWarning("No credentials found on the YubiKey.");
                    return;
                }
                else
                {
                    foreach (RelyingParty relyingParty in relyingParties)
                    {
                        WriteDebug($"Enumerating credentials for {relyingParty.Id}.");
                        IReadOnlyList<CredentialUserInfo> relayCredentials;
                        try
                        {
                            relayCredentials = fido2Session.EnumerateCredentialsForRelyingParty(relyingParty);
                        }
                        catch (NotSupportedException e)
                        {
                            WriteWarning($"Failed to enumerate credentials for {relyingParty.Id}: {e.Message}, SDK might not support algorithm.");
                            continue;
                        }

                        foreach (CredentialUserInfo user in relayCredentials)
                        {
                            if (ParameterSetName == "List-All" || (user.CredentialId.Id.ToArray().SequenceEqual(this.CredentialID!.Value.ToByte())))
                            {
                                Credential credential = new Credential(relyingParty: relyingParty, credentialUserInfo: user);
                                WriteObject(credential);
                            }
                        }
                    }
                }
            }
        }
    }
}