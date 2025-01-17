using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using System.Linq;
using powershellYK.support;
using System.Formats.Cbor;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Yubico.YubiKey.Cryptography;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.New, "YubikeyFIDO2Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubikeyFIDO2CredentialCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Specify which relayingParty (site) this credential is regards to.")]
        public required string RelyingPartyID { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Friendlyname for the relayingParty.")]
        public required string RelyingPartyName { private get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Username to create credental for.")]
        public required string Username { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "UserDisplayName to create credental for.")]
        public string? UserDisplayName { private get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challange.")]
        public required Challenge Challange { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Should this credential be discoverable.")]
        public bool Discoverable { private get; set; } = true;

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
            throw new NotImplementedException("New-YubikeyFIDO2Credential not implemented.");
            if (UserDisplayName is null)
            {
                UserDisplayName = Username;
            }
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                var randomObject = CryptographyProviders.RngCreator();
                byte[] randomBytes = new byte[16];
                randomObject.GetBytes(randomBytes);
                var userId = new ReadOnlyMemory<byte>(randomBytes);
                var relayingParty = new RelyingParty(RelyingPartyID) { Name = RelyingPartyName };
                var user = new UserEntity(userId)
                {
                    Name = Username,
                    DisplayName = UserDisplayName,
                };

                ReadOnlyMemory<byte> clientDataHash = Challange.ToByte().AsMemory();


                var make = new MakeCredentialParameters(relayingParty, user);
                make.AddHmacSecretExtension(fido2Session.AuthenticatorInfo);
                if (Discoverable)
                {
                    make.AddOption("rk", true);
                }

                if (fido2Session.AuthenticatorInfo.IsExtensionSupported("hmac-secret"))
                {
                    make.AddHmacSecretExtension(fido2Session.AuthenticatorInfo);
                }

                make.ClientDataHash = clientDataHash;
                MakeCredentialData returnvalue = fido2Session.MakeCredential(make);
                WriteObject(returnvalue);
            }
        }
    }
}
