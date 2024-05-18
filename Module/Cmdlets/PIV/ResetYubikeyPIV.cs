using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace Yubikey_Powershell.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Reset, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(bool))]
    public class ResetYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Force reset")]
        public SwitchParameter Force { get; set; } = false;
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

            WriteDebug("ProcessRecord in Reset-YubikeyPIV");

            if (ShouldProcess($"Yubikey PIV", "Reset"))
            {
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