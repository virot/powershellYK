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

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._pivSession is null) { throw new Exception("PIV not connected, use Connect-YubikeyPIV first"); }
        }
        protected override void ProcessRecord()
        {
         
            if (PIN.IsPresent)
            {
                int? retriesRemaining = 1;
                Random rnd = new Random();
                while (retriesRemaining > 0)
                {
                    int randomNumber = rnd.Next(0, 99999999);
                    string pinfail = randomNumber.ToString("00000000");
                    byte[] pinfailBytes = Encoding.UTF8.GetBytes(pinfail);
                    YubiKeyModule._pivSession.TryChangePin(pinfailBytes, pinfailBytes, out retriesRemaining);
                }   
            }
            if (PUK.IsPresent)
            {
                int? retriesRemaining = 1;
                Random rnd = new Random();
                while (retriesRemaining > 0)
                {
                    int randomNumber = rnd.Next(0, 99999999);
                    string pukfail = randomNumber.ToString("00000000");
                    byte[] pukfailBytes = Encoding.UTF8.GetBytes(pukfail);
                    YubiKeyModule._pivSession.TryChangePin(pukfailBytes, pukfailBytes, out retriesRemaining);
                }
            }
        }

    }
}