using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace Yubikey_Powershell
{
    [Cmdlet(VerbsSecurity.Block, "YubikeyPIV")]
    public class BlockYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Block the PIN for the PIV device")]
        public SwitchParameter PIN { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Block the PUK for the PIV device")]
        public SwitchParameter PUK { get; set; }

        protected override void ProcessRecord()
        {
            if (YubiKeyModule._connection is null) { throw new Exception("No Yubikey is selected, use Connect-YubikeyPIV first"); }
            WriteDebug("ProcessRecord in Block-YubikeyPIV");

            GetMetadataCommand metadataCommand;
            GetMetadataResponse metadataResponse;

            if (PIN.IsPresent)
            {
                int i, puk_remaining;
                metadataCommand = new GetMetadataCommand(PivSlot.Pin);
                metadataResponse = YubiKeyModule._connection.SendCommand(metadataCommand);
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

                    ChangeReferenceDataCommand blockPukCommand = new ChangeReferenceDataCommand(PivSlot.Pin, pukfailBytes, pukfailBytes);
                    ChangeReferenceDataResponse blockPukResponse = YubiKeyModule._connection.SendCommand(blockPukCommand);
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
                metadataResponse = YubiKeyModule._connection.SendCommand(metadataCommand);
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

                    ChangeReferenceDataCommand blockPukCommand = new ChangeReferenceDataCommand(PivSlot.Puk, pukfailBytes, pukfailBytes);
                    ChangeReferenceDataResponse blockPukResponse = YubiKeyModule._connection.SendCommand(blockPukCommand);
                    if (blockPukResponse.Status == ResponseStatus.Success)
                    {
                        WriteObject("PUK Blocked");
                    }
                }
            }
        }

    }
}