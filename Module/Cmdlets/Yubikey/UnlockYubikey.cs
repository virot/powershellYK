using System.Management.Automation;
using System.Linq;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Unlock, "YubiKey")]
    public class UnlockYubikeyCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Lock Code for YubiKey")]
        public byte[] LockCode { get; set; } = new byte[16];
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                    }
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
            if (YubiKeyModule._yubikey!.ConfigurationLocked == true)
            {
                // Can be used for Yubikeys 5 and later.
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    YubiKeyModule._yubikey!.UnlockConfiguration(LockCode);
                    WriteWarning("Remove and re-insert the YubiKey to undo the lock code...");
                }
            }
        }

    }
}