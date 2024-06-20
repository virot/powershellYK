using System.Management.Automation;
using System.Linq;
using Yubico.YubiKey.Management.Commands;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Lock, "Yubikey")]
    public class LockYubikeyCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "LockCode for Yubikey")]
        public byte[] LockCode { get; set; } = new byte[16];
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        protected override void ProcessRecord()
        {
            if (YubiKeyModule._yubikey!.ConfigurationLocked == false)
            {
                // Can be used for Yubikeys 5 and later.
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    if (!LockCode.SequenceEqual(new byte[] { 0, 0, 0, 0 , 0, 0, 0, 0 , 0, 0, 0, 0 , 0, 0, 0, 0 }))
                    {
                        YubiKeyModule._yubikey!.LockConfiguration(LockCode);
                        YubiKeyModule._yubikey = null;
                        WriteWarning("Please remove and reinsert Yubikey");
                    }
                    else
                    {
                        throw new ArgumentException("Lock code cannot be all zeros");
                    }
                }
            }
            else
            {
                WriteDebug("Yubikey already locked.");
            }
        }

    }
}