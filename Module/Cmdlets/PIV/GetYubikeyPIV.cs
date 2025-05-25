using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using powershellYK.PIV;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Objects;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Get, "YubiKeyPIV")]
    public class GetYubikeyPIVCommand : PSCmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Retrive a info from specific slot")]
        public PIVSlot? Slot { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
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
                if (Slot is null)
                {
                    int pin_retry, pin_remaining, puk_retry, puk_remaining;
                    try
                    {
                        PivMetadata pin = pivSession.GetMetadata(PivSlot.Pin);
                        pin_retry = pin.RetryCount;
                        pin_remaining = pin.RetriesRemaining;
                    }
                    catch
                    {
                        pin_retry = -1;
                        pin_remaining = -1;
                    }
                    try
                    {
                        PivMetadata puk = pivSession.GetMetadata(PivSlot.Puk);
                        puk_retry = puk.RetryCount;
                        puk_remaining = puk.RetriesRemaining;
                    }
                    catch
                    {
                        puk_retry = -1;
                        puk_remaining = -1;
                    }

                    List<PIVSlot> certificateLocations = new List<PIVSlot>();
                    var locationsToCheck = new PIVSlot[] { 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x9a, 0x9c, 0x9d, 0x9e };


                    foreach (var location in locationsToCheck)
                    {
                        try
                        {
                            _ = pivSession.GetMetadata(location).PublicKeyParameters;
                            certificateLocations.Add(location);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    List<string> supportedAlgorithms = new List<string>();

                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa1024)) { supportedAlgorithms.Add("Rsa1024"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa2048)) { supportedAlgorithms.Add("Rsa2048"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa3072)) { supportedAlgorithms.Add("Rsa3072"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivRsa4096)) { supportedAlgorithms.Add("Rsa4096"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP256)) { supportedAlgorithms.Add("EcP256"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivEccP384)) { supportedAlgorithms.Add("EcP384"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivCurve25519)) { supportedAlgorithms.Add("Ed25519"); };
                    if (((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivCurve25519)) { supportedAlgorithms.Add("X25519"); };


                    CardholderUniqueId chuid;
                    try
                    {
                        pivSession.TryReadObject<CardholderUniqueId>(out chuid);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to read CHUID", e);
                    }

                    var customObject = new
                    {
                        PinRetriesLeft = pin_remaining,
                        PinRetries = pin_retry,
                        PukRetriesLeft = puk_remaining,
                        PukRetries = puk_retry,
                        CHUID = BitConverter.ToString(chuid.GuidValue.Span.ToArray()),
                        SlotsWithPrivateKeys = certificateLocations.ToArray(),
                        //PinVerified = pivSession.PinVerified, // This was false even after a successful pin verification
                        PinVerified = (YubiKeyModule._pivPIN is not null),
                        ManagementkeyPIN = pivSession.GetPinOnlyMode(),
                        SupportedAlgorithms = supportedAlgorithms,
                    };

                    WriteObject(customObject);
                }
                else
                {
                    try
                    {
                        X509Certificate2? certificate = null;
                        PivMetadata slotData = pivSession.GetMetadata((byte)Slot);

                        //using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(slotData.PublicKey);

                        try { certificate = pivSession.GetCertificate((byte)Slot); } catch { }

                        SlotInfo returnSlot = new SlotInfo(
                            slotData.Slot,
                            slotData.KeyStatus,
                            slotData.Algorithm,
                            slotData.PinPolicy,
                            slotData.TouchPolicy,
                            certificate
                            );
                        WriteObject(returnSlot);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to get metadata from slot {Slot}", e);
                    }

                }

            }
        }

    }
}
