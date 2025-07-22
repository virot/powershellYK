/// <summary>
/// Finds and returns YubiKey(s) connected to the system.
/// Can filter results to return only one YubiKey or a specific YubiKey by serial number.
/// 
/// .EXAMPLE
/// Find-YubiKey
/// Returns all connected YubiKeys
/// 
/// .EXAMPLE
/// Find-YubiKey -OnlyOne
/// Returns only the first connected YubiKey
/// 
/// .EXAMPLE
/// Find-YubiKey -Serialnumber 1234567
/// Returns a specific YubiKey by serial number
/// </summary>

// Imports
using powershellYK.YubiKey;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Find, "YubiKey")]
    public class FindYubikeyCommand : Cmdlet
    {
        // Parameters for YubiKey filtering
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only one YubiKey")]
        public SwitchParameter OnlyOne { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only YubiKey with Serial Number")]
        public int? Serialnumber { get; set; }

        // Process the main cmdlet logic
        protected override void BeginProcessing()
        {
            WriteDebug("ProcessRecord in Get-Yubikey");

            // Handle serial number filtering
            if (Serialnumber is not null)
            {
                // Find specific YubiKey by serial number
                IYubiKeyDevice yubiKey;
                if (YubiKeyDevice.TryGetYubiKey((int)Serialnumber, out yubiKey))
                {
                    WriteObject((YubiKeyDevice)yubiKey);
                }
                else
                {
                    throw new Exception($"The specific YubiKey ({Serialnumber}) was not found.");
                }
            }
            else
            {
                // Find all connected YubiKeys
                IEnumerable<IYubiKeyDevice> yubiKeys = YubiKeyDevice.FindAll();

                // Return found YubiKeys
                foreach (var yubiKey in yubiKeys)
                {
                    WriteObject(new YubikeyInformation(yubiKey: (YubiKeyDevice)yubiKey));
                }

                // Handle case when no YubiKeys are found
                if (yubiKeys.ToArray().Length == 0)
                {
                    WriteWarning("No YubiKeys found, FIDO-only YubiKeys on Windows requires running as Administrator.");
                }
            }
        }
    }
}