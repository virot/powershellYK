/// <summary>
/// Unlocks the configuration of a YubiKey.
/// Allows modification of device settings after providing the correct lock code.
/// Requires a YubiKey 5 or later with a locked configuration.
/// 
/// .EXAMPLE
/// $lockCode = [byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
/// Unlock-YubiKey -LockCode $lockCode
/// Unlocks the YubiKey configuration using the provided lock code
/// </summary>

// Imports
using System.Management.Automation;
using System.Linq;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Unlock, "YubiKey")]
    public class UnlockYubikeyCommand : PSCmdlet
    {
        // Parameters for lock code
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Lock Code for YubiKey")]
        public byte[] LockCode { get; set; } = new byte[16];

        // Initialize processing
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    // Create a new PowerShell instance to run Connect-Yubikey
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

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Check if configuration is locked
            if (YubiKeyModule._yubikey!.ConfigurationLocked == true)
            {
                // Verify YubiKey version
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    // Unlock the configuration
                    YubiKeyModule._yubikey!.UnlockConfiguration(LockCode);
                    WriteWarning("Remove and re-insert the YubiKey to undo the lock code...");
                }
            }
        }
    }
}