﻿using System.Management.Automation;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Reset, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(bool))]
    public class ResetYubikeyPIVCommand : Cmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Force reset of the PIV applet")]
        public SwitchParameter Force { get; set; } = false;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        protected override void ProcessRecord()
        {
            WriteDebug("ProcessRecord in Reset-YubikeyPIV");
            if (Force || ShouldProcess("This will delete all PIV credentials, and restore factory settings. Proceed?", "This will delete all PIV credentials, and restore factory settings. Proceed?", "WARNING!"))
            {
                using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    pivSession.ResetApplication();
                }
                WriteInformation("YubiKey PIV applet successfully reset.", new string[] { "PIV", "Reset" });
            }
        }
    }
}
