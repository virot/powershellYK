/// <summary>
/// Creates a new key pair in a specified YubiKey PIV slot.
/// Supports various algorithms and policies for PIN and touch requirements.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// New-YubiKeyPIVKey -Slot "PIV Authentication" -Algorithm "Rsa2048"
/// Creates a new RSA 2048 key in the PIV Authentication slot
/// 
/// .EXAMPLE
/// New-YubiKeyPIVKey -Slot "Digital Signature" -Algorithm "EcP256" -PinPolicy "Never" -TouchPolicy "Always"
/// Creates a new ECC P-256 key in the Digital Signature slot with custom policies
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using System.Collections.ObjectModel;
using powershellYK.support.validators;
using powershellYK.PIV;
using Yubico.YubiKey.Cryptography;
using powershellYK.support;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubiKeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVKeyCommand : PSCmdlet, IDynamicParameters
    {
        // Parameters for key creation
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [ValidateYubikeyPIVSlot(DontAllowAttestion = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key in")]
        public PIVSlot Slot { get; set; }

        // Parameters for key policies
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Pin policy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Touch policy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        // Parameters for output control
        [Parameter(Mandatory = false, HelpMessage = "Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.")]
        public SwitchParameter PassThru { get; set; }

        // Get dynamic parameters based on YubiKey capabilities
        public object GetDynamicParameters()
        {
            // Get available algorithms from YubiKey
            var availableAlgorithms = new List<String>();
            try { YubiKeyModule.ConnectYubikey(); } catch { }
            if (YubiKeyModule._yubikey is not null)
            {
                // Check for supported RSA algorithms
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa1024)) { availableAlgorithms.Add("Rsa1024"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa2048)) { availableAlgorithms.Add("Rsa2048"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa3072)) { availableAlgorithms.Add("Rsa3072"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa4096)) { availableAlgorithms.Add("Rsa4096"); };
                
                // Check for supported ECC algorithms
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP256)) { availableAlgorithms.Add("EcP256"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP384)) { availableAlgorithms.Add("EcP384"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivCurve25519)) { availableAlgorithms.Add("Ed25519"); };
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivCurve25519)) { availableAlgorithms.Add("X25519"); };
            }
            else
            {
                // Default algorithms if no YubiKey is connected
                availableAlgorithms.Add("Rsa1024");
                availableAlgorithms.Add("Rsa2048");
                availableAlgorithms.Add("Rsa3072");
                availableAlgorithms.Add("Rsa4096");
                availableAlgorithms.Add("EcP256");
                availableAlgorithms.Add("EcP384");
                availableAlgorithms.Add("Ed25519");
                availableAlgorithms.Add("X25519");
            }

            // Create dynamic parameter for algorithm selection
            var runtimeDefinedParameterDictionary = new RuntimeDefinedParameterDictionary();
            var algorithmCollection = new Collection<Attribute>() {
                new ParameterAttribute() { Mandatory = true, HelpMessage = "What algorithm to use, dependent on YubiKey firmware.", ParameterSetName = "__AllParameterSets", ValueFromPipeline = false },
                new ValidateSetAttribute((availableAlgorithms).ToArray())};
            var runtimeDefinedAlgorithms = new RuntimeDefinedParameter("Algorithm", typeof(KeyType), algorithmCollection);
            runtimeDefinedParameterDictionary.Add("Algorithm", runtimeDefinedAlgorithms);
            return runtimeDefinedParameterDictionary;
        }

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            YubiKeyModule.ConnectYubikey();
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Open a session with the YubiKey PIV application
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Check if key already exists in slot
                bool keyExists = false;
                try
                {
                    IPublicKey? pubkey = pivSession.GetMetadata(Slot).PublicKeyParameters;
                    keyExists = true;
                }
                catch { }

                // Generate new key if slot is empty or user confirms
                if (!keyExists || ShouldProcess($"Slot {Slot}", "New"))
                {
                    // Generate key pair with specified policies
                    IPublicKey publicKey = pivSession.GenerateKeyPair(Slot, (KeyType)this.MyInvocation.BoundParameters["Algorithm"], PinPolicy, TouchPolicy);
                    if (publicKey is not null)
                    {
                        if (PassThru.IsPresent)
                        {
                            WriteObject(publicKey);
                        }
                        WriteInformation($"New key(s) created in slot {Slot}.", new string[] { "PIV", "Info" });
                    }
                    else
                    {
                        throw new Exception("Could not create keypair!");
                    }
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
