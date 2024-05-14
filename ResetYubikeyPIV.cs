using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace Virot.Yubikey
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(bool))]
    public class ResetYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Reset a specific Yubikey by Serialnumber")]
        public YubiKeyDevice? YubiKey {
            get { return _yubikey; }
            set { _yubikey = value; }
        }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Force reset")]
        public SwitchParameter Force { get; set; } = false;
        private YubiKeyDevice? _yubikey;
        private IYubiKeyConnection? _connection = null;

        protected override void BeginProcessing()
        {
            WriteDebug("BeginProcessing in Reset-YubikeyPIV");
            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]             //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]
            if (YubiKey is null)
            {
                //declare a variable called temp_yubikey of the type YubikeyDevice                //declare a variable called temp_yubikey of the type YubikeyDevice                //declare a variable called temp_yubikey of the type YubikeyDevice

                //get all yubikeys                //get all yubikeys
                //populate the yubikeys variable from the function GetYubiKey

                GetYubikeyCommand gy = new GetYubikeyCommand
                {
                    OnlyOne = true
                };
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
            WriteDebug("ProcessRecord in Reset-YubikeyPIV");

            if (ShouldProcess($"Yubikey serialnumber {_yubikey.SerialNumber}","Reset")) {
                /*
                 * BlockYubikeyPIVCommand blockPIVPINPUK = new BlockYubikeyPIVCommand
                {
                    PIN = true,
                    PUK = true
                };
                //if debug is set, make sure that blockpinpuk is also set to debug
                if ()
                {
                    //add debug to blockpinpuk                    //add debug to blockpinpuk
                    //blockPIVPINPUK.CommonParameters.Add("Debug");
                    WriteObject("Debug is set");
                }
                blockPIVPINPUK.Invoke();
                */

                ResetPivCommand resetPivCmd = new ResetPivCommand();
                ResetPivResponse resetPivResp = _connection.SendCommand(resetPivCmd);
                if (resetPivResp.Status == ResponseStatus.Success)
                {
                    WriteObject(true);
                }
                else
                {
                    throw new Exception("Failed to reset Yubikey PIV");
                }


            }
            /*
            if (PIN.IsPresent)
            {
                int i, puk_remaining;
                metadataCommand = new GetMetadataCommand(PivSlot.Pin);
                metadataResponse = _connection.SendCommand(metadataCommand);
                if (metadataResponse.Status == ResponseStatus.Success)
                {
                    puk_remaining = metadataResponse.GetData().RetriesRemaining;
                }
                else
                {
                    throw new Exception("Failed to request number of remaining tries");
                }

                //create a random number to fail with                //create a random number to fail with
                Random rnd = new Random();
                int randomNumber = rnd.Next(0, 99999999);
                for (i = 0; i <= puk_remaining; i++)
                {
                    string pukfail = (randomNumber + i).ToString("00000000");
                    WriteDebug("Trying PUK: " + pukfail);
                    // convert pukfail to byte array                    // conv                    // convert pukfail to byte array                    // convert pukfail to byte array
                    byte[] pukfailBytes = Encoding.UTF8.GetBytes(pukfail);

                    ChangeReferenceDataCommand blockPukCommand = new Yubico.YubiKey.Piv.Commands.ChangeReferenceDataCommand(PivSlot.Pin, pukfailBytes, pukfailBytes);
                    ChangeReferenceDataResponse blockPukResponse = _connection.SendCommand(blockPukCommand);
                    if (blockPukResponse.Status == ResponseStatus.Success)
                    {
                        WriteObject("PUK Blocked");
                    }
                }
            }
            if (PUK.IsPresent)
            {
                int i, puk_remaining;
                metadataCommand = new GetMetadataCommand(PivSlot.Puk);
                metadataResponse = _connection.SendCommand(metadataCommand);
                if (metadataResponse.Status == ResponseStatus.Success)
                {
                    puk_remaining = metadataResponse.GetData().RetriesRemaining;
                }
                else
                {
                    throw new Exception("Failed to request number of remaining tries");
                }

                //create a random number to fail with                //create a random number to fail with
                Random rnd = new Random();
                int randomNumber = rnd.Next(0, 99999999);
                for (i = 0; i <= puk_remaining; i++)
                {
                    string pukfail = (randomNumber + i).ToString("00000000");
                    WriteDebug("Trying PUK: " + pukfail);
                    // convert pukfail to byte array                    // conv                    // convert pukfail to byte array                    // convert pukfail to byte array
                    byte[] pukfailBytes = Encoding.UTF8.GetBytes(pukfail);

                    ChangeReferenceDataCommand blockPukCommand = new Yubico.YubiKey.Piv.Commands.ChangeReferenceDataCommand(PivSlot.Puk, pukfailBytes, pukfailBytes);
                    ChangeReferenceDataResponse blockPukResponse = _connection.SendCommand(blockPukCommand);
                    if (blockPukResponse.Status == ResponseStatus.Success)
                    {
                        WriteObject("PUK Blocked");
                    }
                }
            }
            */
        }


        protected override void EndProcessing()
        {
            WriteDebug("EndProcessing in Reset-YubikeyPIV");
            if (_connection is not null)
            {
                _connection.Dispose();
            }
        }
    }
}