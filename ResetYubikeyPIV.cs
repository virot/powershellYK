using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubikey_Powershell;


namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(bool))]
    public class ResetYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Force reset")]
        public SwitchParameter Force { get; set; } = false;
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._pivSession is null) { throw new Exception("PIV not connected, use Connect-YubikeyPIV first"); }
        }

        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord in Reset-YubikeyPIV");

            if (ShouldProcess($"Yubikey serialnumber {YubiKeyModule._yubikey.SerialNumber}", "Reset"))
            {
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
                try
                {
                    YubiKeyModule._pivSession.ResetApplication();
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to reset Yubikey PIV", e);
                }


            }
        }
    }
}