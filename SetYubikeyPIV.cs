using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommon.Set, "YubikeyPIV")]
    public class SetYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Change number of PIN retries")]
        public byte? PinRetries { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Change number of PUK retries")]
        public byte? PukRetries { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current PIN")]
        public string? PIN { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "New PIN")]
        public string? NewPIN { get; set; }

        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord in Set-YubikeyPIV");
            if (YubiKeyModule._connection is null) { throw new Exception("No Yubikey is selected, use Connect-YubikeyPIV first"); }

            if ((PinRetries is not null) && (PukRetries is not null))
            {
                SetPinRetriesCommand setPinRetries = new SetPinRetriesCommand((byte)PinRetries, (byte)PukRetries);
                SetPinRetriesResponse setPinRetriesResponse = YubiKeyModule._connection.SendCommand(setPinRetries);
                if (setPinRetriesResponse.Status == ResponseStatus.Success)
                {
                    WriteObject(true);
                }
                else if (setPinRetriesResponse.Status == ResponseStatus.AuthenticationRequired)
                {
                    throw new Exception("Authentication required to set PIN and PUK retries");
                }
                else
                {
                    throw new Exception("Failed to set PIN and PUK retries");
                }
            }
            if ((PIN is not null) && (NewPIN is not null))
            {
                byte[] pinarray = System.Text.Encoding.UTF8.GetBytes(PIN);
                byte[] newpinarray = System.Text.Encoding.UTF8.GetBytes(NewPIN);
                ChangeReferenceDataCommand changePin = new ChangeReferenceDataCommand(PivSlot.Pin,pinarray, newpinarray);
                ChangeReferenceDataResponse changePinResponse = YubiKeyModule._connection.SendCommand(changePin);
                if (changePinResponse.Status == ResponseStatus.Success)
                {
                    WriteObject(true);
                }
                else if (changePinResponse.Status == ResponseStatus.AuthenticationRequired)
                {
                    throw new Exception("Authentication required to change PIN");
                }
                else
                {
                    throw new Exception("Failed to change PIN");
                }
            
            }
        }
    }
}