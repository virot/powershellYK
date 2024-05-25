using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Objects;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Get, "YubikeyPIV")]
    public class GetYubikeyPIVCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Retrive a info from specific slot")]
        public byte? Slot { get; set; }

        protected override void ProcessRecord()
        {
            if (YubiKeyModule._pivSession is null)
            {
                //throw new Exception("PIV not connected, use Connect-YubikeyPIV first");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyPIV");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            if (Slot is null)
            {
                int pin_retry, pin_remaining, puk_retry, puk_remaining;
                try
                {
                    PivMetadata pin = YubiKeyModule._pivSession!.GetMetadata(PivSlot.Pin);
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
                    PivMetadata puk = YubiKeyModule._pivSession!.GetMetadata(PivSlot.Puk);
                    puk_retry = puk.RetryCount;
                    puk_remaining = puk.RetriesRemaining;
                }
                catch
                {
                    puk_retry = -1;
                    puk_remaining = -1;
                }

                List<byte> certificateLocations = new List<byte>();
                byte[] locationsToCheck = new byte[] { 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x9a, 0x9c, 0x9d, 0x9e };

                foreach (byte location in locationsToCheck)
                {
                    try
                    {
                        PivPublicKey pubkey = YubiKeyModule._pivSession!.GetMetadata(location).PublicKey;
                        certificateLocations.Add(location);
                    }
                    catch
                    {
                        continue;
                    }
                }

                byte[] certificateLocationsArray = certificateLocations.ToArray();

                CardholderUniqueId chuid;
                try
                {
                    YubiKeyModule._pivSession!.TryReadObject<CardholderUniqueId>(out chuid);
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
                    SlotsWithPrivateKeys = certificateLocationsArray
                };

                WriteObject(customObject);
            }
            else
            {
                try
                {
                    X509Certificate2? certificate = null;
                    PivMetadata slotData = YubiKeyModule._pivSession!.GetMetadata((byte)Slot);
                    using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(slotData.PublicKey);

                    try { certificate = YubiKeyModule._pivSession.GetCertificate((byte)Slot); } catch { }
                    var customObject = new
                    {
                        Slot = slotData.Slot,
                        Algorithm = slotData.Algorithm,
                        KeyStatus = slotData.KeyStatus,
                        PinPolicy = slotData.PinPolicy,
                        TouchPolicy = slotData.TouchPolicy,
                        Certificate = certificate,
                        PublicKey = dotNetPublicKey,
                    };
                    WriteObject(customObject);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get metadata from slot {Slot}", e);
                }

            }

        }


    }
}