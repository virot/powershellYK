/// <summary>
/// Retrieves detailed information about the currently connected YubiKey.
/// Returns YubiKey information including serial number, firmware version, and capabilities.
/// Automatically attempts to connect to a YubiKey if none is currently connected.
/// 
/// .EXAMPLE
/// Get-YubiKey
/// Returns information about the currently connected YubiKey
/// </summary>

using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using powershellYK.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Get, "YubiKey")]
    public class GetYubikeyCommand : PSCmdlet
    {
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
                    if (this.MyInvocation.BoundParameters.ContainsKey("ErrorAction"))
                    {
                        myPowersShellInstance = myPowersShellInstance.AddParameter("ErrorAction", this.MyInvocation.BoundParameters["ErrorAction"]);
                    }
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Connect-Yubikey completed.");
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
            // Check if a YubiKey is connected
            if (YubiKeyModule._yubikey is not null)
            {
                try
                {
                    // Get and return YubiKey information
                    WriteObject(new YubikeyInformation(yubiKey: (YubiKeyDevice)YubiKeyDevice.FindAll().Where(yk => yk.SerialNumber == YubiKeyModule._yubikey!.SerialNumber).First()));
                }
                catch (System.InvalidOperationException e)
                {
                    WriteError(new ErrorRecord(new Exception("Failed to load the YubiKey information", e), "0x00020002", ErrorCategory.InvalidResult, null));
                }
            }
            else
            {
                WriteError(new ErrorRecord(new Exception("None YubiKeys selected, Use Connect-Yubikey to specify which Yubikey to use."), "0x00020001", ErrorCategory.InvalidResult, null));
            }
        }
    }
}