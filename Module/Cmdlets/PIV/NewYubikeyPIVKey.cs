using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVKeyCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key for")]
        public byte Slot { get; set; }

        [ValidateSet("Rsa1024", "Rsa2048", "EccP256", "EccP384", IgnoreCase = true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; }

        [ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [Parameter(Mandatory = false, HelpMessage = "Returns an object that represents the item with which you're working. By default, this cmdlet doesn't generate any output.")]
        public SwitchParameter PassThru { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
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

                if (!keyExists || ShouldProcess($"Slot 0x{Slot.ToString("X2")}", "New"))
                {
                    WriteDebug("ProcessRecord in New-YubikeyPIVKey");
                    PivPublicKey publicKey = pivSession.GenerateKeyPair(Slot, Algorithm, PinPolicy, TouchPolicy);
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
                    }
                    else
                    {
                        throw new Exception("Could not create keypair");
                    }
                }
            }
        }
    }
}