/// <summary>
/// Locks the configuration of a YubiKey.
/// Prevents unauthorized modification of YubiKey settings by requiring a lock code.
/// Requires a YubiKey 5 or later with an unlocked configuration.
/// 
/// .EXAMPLE
/// $lockCode = [byte[]]@(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16)
/// Lock-YubiKey -LockCode $lockCode
/// Locks the YubiKey configuration using the provided lock code
/// </summary>

// Imports
using System.Management.Automation;
using System.Linq;
using Yubico.YubiKey.Management.Commands;

namespace powershellYK.Cmdlets.OTP
{
    [Cmdlet(VerbsCommon.Lock, "YubiKey")]
    public class LockYubikeyCommand : PSCmdlet
    {
        // Parameters for lock code
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "LockCode for Yubikey")]
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
            if (YubiKeyModule._yubikey!.ConfigurationLocked == false)
            {
                // Verify YubiKey version
                if (YubiKeyModule._yubikey!.FirmwareVersion.Major >= 5)
                {
                    // Validate lock code
                    if (!LockCode.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }))
                    {
                        // Lock the configuration
                        YubiKeyModule._yubikey!.LockConfiguration(LockCode);
                        YubiKeyModule._yubikey = null;
                        WriteWarning("Remove and re-insert the YubiKey to set lock code...");
                    }
                    else
                    {
                        throw new ArgumentException("Lock code cannot be all zeros!");
                    }
                }
            }
            else
            {
                WriteDebug("Yubikey already locked!");
            }
        }
    }
}