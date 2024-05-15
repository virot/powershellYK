using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubikey_Powershell;


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
        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "Current PUK")]
        public string? PUK { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK")]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "New PUK")]
        public string? NewPUK { get; set; }

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._pivSession is null) { throw new Exception("PIV not connected, use Connect-YubikeyPIV first"); }
        }
        protected override void ProcessRecord()
        {
         
            if ((PinRetries is not null) && (PukRetries is not null))
            {
                try
                {
                    YubiKeyModule._pivSession.ChangePinAndPukRetryCounts((byte)PinRetries, (byte)PukRetries);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to set PIN and PUK retries", e);
                }
            }
            if ((PIN is not null) && (NewPIN is not null))
            {
                byte[] pinarray = System.Text.Encoding.UTF8.GetBytes(PIN);
                byte[] newpinarray = System.Text.Encoding.UTF8.GetBytes(NewPIN);
                int? retriesLeft = null;
                try
                {
                    YubiKeyModule._pivSession.TryChangePin(pinarray, newpinarray, out retriesLeft);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to change PIN", e);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(pinarray);
                    CryptographicOperations.ZeroMemory(newpinarray);
                }
            }
            if ((PUK is not null) && (NewPUK is not null))
            {
                byte[] pukarray = System.Text.Encoding.UTF8.GetBytes(PUK);
                byte[] newpukarray = System.Text.Encoding.UTF8.GetBytes(NewPUK);
                int? retriesLeft = null;
                try
                {
                    YubiKeyModule._pivSession.TryChangePuk(pukarray, newpukarray, out retriesLeft);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to change PUK", e);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(pukarray);
                    CryptographicOperations.ZeroMemory(newpukarray);
                }
            }
        }
    }
}