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
            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]             //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]            //if there is no yubikey sent in, use the get-yubikey function, if that returns more than one yubikey, throw [eu.virot.yubikey.multiplefound]
            if (YubiKeyModule._connection is null) { throw new Exception("No Yubikey is selected, use Connect-YubikeyPIV first"); }
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

                ResetPivCommand resetPivCmd = new ResetPivCommand();
                ResetPivResponse resetPivResp = YubiKeyModule._connection.SendCommand(resetPivCmd);
                if (resetPivResp.Status == ResponseStatus.Success)
                {
                    YubiKeyModule._connection.Dispose();
                    YubiKeyModule._connection = null;
                    WriteObject(true);
                }
                else
                {
                    throw new Exception("Failed to reset Yubikey PIV");
                }


            }
        }
    }
}