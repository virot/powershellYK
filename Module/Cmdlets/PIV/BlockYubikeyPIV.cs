using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsSecurity.Block, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class BlockYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = true, ParameterSetName = "BlockBoth")]
        [Parameter(Mandatory = true, ParameterSetName = "BlockPIN")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Block the PIN for the PIV device")]
        public SwitchParameter PIN { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "BlockBoth")]
        [Parameter(Mandatory = true, ParameterSetName = "BlockPUK")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Block the PUK for the PIV device")]
        public SwitchParameter PUK { get; set; }

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

            if (PIN.IsPresent)
            {
                try
                {
                    int? retriesRemaining = 1;
                    Random rnd = new Random();
                    while (retriesRemaining > 0)
                    {
                        int randomNumber = rnd.Next(0, 99999999);
                        string pinfail = randomNumber.ToString("00000000");
                        byte[] pinfailBytes = Encoding.UTF8.GetBytes(pinfail);
                        YubiKeyModule._pivSession!.TryChangePin(pinfailBytes, pinfailBytes, out retriesRemaining);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message != "There are no retries remaining for a PIN, PUK, or other authentication element.")
                    {
                        throw new Exception("Failed to block PUK", e);
                    }
                }
            }
            if (PUK.IsPresent)
            {
                try
                {
                    int? retriesRemaining = 1;
                    Random rnd = new Random();
                    while (retriesRemaining > 0)
                    {
                        int randomNumber = rnd.Next(0, 99999999);
                        string pukfail = randomNumber.ToString("00000000");
                        byte[] pukfailBytes = Encoding.UTF8.GetBytes(pukfail);
                        YubiKeyModule._pivSession!.TryChangePuk(pukfailBytes, pukfailBytes, out retriesRemaining);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message != "There are no retries remaining for a PIN, PUK, or other authentication element.") {
                        throw new Exception("Failed to block PUK", e);
                    }
                }
            }
        }

    }
}