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
    [Cmdlet(VerbsCommon.New, "YubiKeyFIDO2Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubikeyFIDO2CredentialCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Specify which relayingParty (site) this credential is regards to.", ParameterSetName = "UserEntity-HostData")]
        public required string RelyingPartyID { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Friendlyname for the relayingParty.", ParameterSetName = "UserEntity-HostData")]
        public required string RelyingPartyName { private get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelaingParty object.", ParameterSetName = "UserData-RelyingParty")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "RelaingParty object.", ParameterSetName = "UserEntity-RelyingParty")]
        public required RelyingParty RelyingParty { private get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Username to create credental for.", ParameterSetName = "UserData-HostData")]
        public required string Username { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "UserDisplayName to create credental for.", ParameterSetName = "UserData-HostData")]
        public string? UserDisplayName { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "UserID.", ParameterSetName = "UserData-HostData")]
        public byte[]? UserID { private get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Challange.")]
        public required Challenge Challenge { private get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Should this credential be discoverable.")]
        public bool Discoverable { private get; set; } = true;

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Supply the user entity in complete form.", ParameterSetName = "UserEntity-HostData")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Supply the user entity in complete form.", ParameterSetName = "UserEntity-RelyingParty")]
        public UserEntity? UserEntity { get; set; } = new UserEntity(new byte[] { 0, 0 });
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
            WriteWarning("This cmdlet is still in development and may not work as expected.");
            if (UserDisplayName is null)
            {
                UserDisplayName = Username;
            }
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                //var randomObject = CryptographyProviders.RngCreator();
                //byte[] randomBytes = new byte[32];
                //randomObject.GetBytes(UserID);
                var userId = new ReadOnlyMemory<byte>(UserID);

                if (RelyingParty is null)
                {
                    RelyingParty = new RelyingParty(RelyingPartyID) { Name = RelyingPartyName };
                }
                if (UserEntity is null)
                {
                    UserEntity = new UserEntity(userId)
                    {
                        Name = Username,
                        DisplayName = UserDisplayName ?? Username,
                    };
                }

 

                var make = new MakeCredentialParameters(RelyingParty, UserEntity);
                if (Discoverable)
                {
                    make.AddOption("rk", true);
                }

                if (fido2Session.AuthenticatorInfo.IsExtensionSupported("hmac-secret"))
                {
                    make.AddHmacSecretExtension(fido2Session.AuthenticatorInfo);
                }

                //ReadOnlyMemory<byte> clientDataHash = Challange.ToByte().AsMemory();
                make.ClientDataHash = Challenge.CalculateSHA256().AsMemory();

                MakeCredentialData returnvalue = fido2Session.MakeCredential(make);
                WriteObject(returnvalue);
            }
        }
    }
}
