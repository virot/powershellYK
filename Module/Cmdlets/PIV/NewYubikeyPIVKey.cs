using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using System.Collections.ObjectModel;
using powershellYK.support.validators;
using powershellYK.PIV;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubiKeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVKeyCommand : PSCmdlet, IDynamicParameters
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key in")]
        public PIVSlot Slot { get; set; }

        //[ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Pin policy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Touch policy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [Parameter(Mandatory = false, HelpMessage = "Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.")]
        public SwitchParameter PassThru { get; set; }

        public object GetDynamicParameters()
        {
            //Add the Algorithm parameter
            var availableAlgorithms = new List<String>();
            try { YubiKeyModule.ConnectYubikey(); } catch { }
            if (YubiKeyModule._yubikey is not null)
            {
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa1024)) { availableAlgorithms.Add("Rsa1024"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa2048)) { availableAlgorithms.Add("Rsa2048"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa3072)) { availableAlgorithms.Add("Rsa3072"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa4096)) { availableAlgorithms.Add("Rsa4096"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP256)) { availableAlgorithms.Add("EccP256"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP384)) { availableAlgorithms.Add("EccP384"); };
            }
            else
            {
                //if no yubikey is attatched, then just list all that we know of.
                availableAlgorithms.Add("Rsa1024");
                availableAlgorithms.Add("Rsa2048");
                availableAlgorithms.Add("Rsa3072");
                availableAlgorithms.Add("Rsa4096");
                availableAlgorithms.Add("EccP256");
                availableAlgorithms.Add("EccP384");
            }
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();

            var algorithmCollection = new Collection<Attribute>() {
                new ParameterAttribute() { Mandatory = true, HelpMessage = "What algorithm to use, dependent on YubiKey firmware.", ParameterSetName = "__AllParameterSets", ValueFromPipeline = false },
                new ValidateSetAttribute((availableAlgorithms).ToArray())};
            var runtimeDefinedAlgorithms = new RuntimeDefinedParameter("Algorithm", typeof(PivAlgorithm), algorithmCollection);
            runtimeDefinedParameterDictionary.Add("Algorithm", runtimeDefinedAlgorithms);
            return runtimeDefinedParameterDictionary;
        }
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                bool keyExists = false;
                try
                {
                    PivPublicKey pubkey = pivSession.GetMetadata(Slot).PublicKey;
                    keyExists = true;
                }
                catch { }

                if (!keyExists || ShouldProcess($"Slot {Slot}", "New"))
                {
                    WriteDebug("ProcessRecord in New-YubikeyPIVKey");
                    PivPublicKey publicKey = pivSession.GenerateKeyPair(Slot, (PivAlgorithm)this.MyInvocation.BoundParameters["Algorithm"], PinPolicy, TouchPolicy);
                    if (publicKey is not null)
                    {
                        if (PassThru.IsPresent)
                        {
                            using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);
                            if (publicKey is PivRsaPublicKey)
                            {
                                WriteObject((RSA)dotNetPublicKey);
                            }
                            else
                            {
                                WriteObject((ECDsa)dotNetPublicKey);
                            }
                        }
                        WriteInformation($"New private key created in slot {Slot}.", new string[] { "PIV", "Info" });
                    }
                    else
                    {
                        throw new Exception("Could not create keypair!");
                    }
                }
            }
        }
    }
}
