using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace Virot.Yubikey
{
    [Cmdlet(VerbsSecurity.Block, "YubikeyPIV")]
    public class BlockYubikeyPIV : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Retrive a specific Yubikey by Serialnumber")]
        public YubiKeyDevice? YubiKey {
            get { return _yubikey; }
            set { _yubikey = value; }
        }
        
        private YubiKeyDevice? _yubikey;
        private IYubiKeyConnection? _connection = null;

        protected override void BeginProcessing()
        {
            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]             //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]
            if (YubiKey is null)
            {
                //declare a variable called temp_yubikey of the type YubikeyDevice                //declare a variable called temp_yubikey of the type YubikeyDevice                //declare a variable called temp_yubikey of the type YubikeyDevice

                //get all yubikeys                //get all yubikeys
                //populate the yubikeys variable from the function GetYubiKey

                GetYubikeyCommand gy = new GetYubikeyCommand();
                gy.OnlyOne = true;
                try
                {
                    var yubiKeys = gy.Invoke<YubiKeyDevice>();
                    _yubikey = (YubiKeyDevice?)yubiKeys.First();
                }
                catch (ItemNotFoundException e)
                {
                    throw new ItemNotFoundException("No Yubikey found", e);
                }
                catch
                {
                    throw new Exception("Multiple Yubikeys found");
                }
            }
            try
            {
                _connection = _yubikey.Connect(YubiKeyApplication.Piv);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Yubikey", e);
            }
        }

        protected override void ProcessRecord()
        {
                //build a custom object to be returned with the following properties: Pin retries left, pin retries, puk retries left, puk retries
                int pin_retry, pin_remaining, puk_retry, puk_remaining;
                GetMetadataCommand metadataCommand = new GetMetadataCommand(0x81);
                GetMetadataResponse metadataResponse = _connection.SendCommand(metadataCommand);

                if (metadataResponse.Status == ResponseStatus.Success)
                {
                    pin_retry = metadataResponse.GetData().RetryCount;
                    pin_remaining = metadataResponse.GetData().RetriesRemaining;
                }
                else
                {
                    pin_retry = -1;
                    pin_remaining = -1;
                }
                metadataCommand = new GetMetadataCommand(0x81);
                metadataResponse = _connection.SendCommand(metadataCommand);
                if (metadataResponse.Status == ResponseStatus.Success)
                {
                    puk_retry = metadataResponse.GetData().RetryCount;
                    puk_remaining = metadataResponse.GetData().RetriesRemaining;
                }
                else
                {
                    puk_retry = -1;
                    puk_remaining = -1;
                }

                List<byte> certificateLocations = new List<byte>();
                byte[] locationsToCheck = new byte[] { 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x9a, 0x9c, 0x9d, 0x9e };

                foreach (byte location in locationsToCheck)
                {
                    metadataCommand = new GetMetadataCommand(location);
                    metadataResponse = _connection.SendCommand(metadataCommand);
                    if (metadataResponse.Status == ResponseStatus.Success)
                    {
                        certificateLocations.Add(location);
                    }
                }

                byte[] certificateLocationsArray = certificateLocations.ToArray();

                var customObject = new
                {
                    PinRetriesLeft = pin_remaining,
                    PinRetries = pin_retry,
                    PukRetriesLeft = puk_remaining,
                    PukRetries = puk_retry,
                    SlotsWithCertificats = certificateLocationsArray
                };

                WriteObject(customObject);

        }


        protected override void EndProcessing()
        {
            if (_connection is not null)
            {
                _connection.Dispose();
            }
        }
    }
}