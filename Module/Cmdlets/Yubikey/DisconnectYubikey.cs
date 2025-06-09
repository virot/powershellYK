/// <summary>
/// Disconnects from the currently connected YubiKey.
/// Clears the YubiKey connection and any stored credentials.
/// 
/// .EXAMPLE
/// Disconnect-YubiKey
/// Disconnects from the currently connected YubiKey
/// </summary>

// Imports
using powershellYK.support;
using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Disconnect, "YubiKey")]
    public class DisconnectYubikeyCommand : Cmdlet
    {
        // Process the main cmdlet logic
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected
            if (YubiKeyModule._yubikey is not null)
            {
                // Get YubiKey information for disconnect message
                if (YubiKeyModule._yubikey.SerialNumber is not null)
                {
                    WriteInformation($"Disconnected from {PowershellYKText.FriendlyName(YubiKeyModule._yubikey)} with serial: {YubiKeyModule._yubikey.SerialNumber}.", new string[] { "YubiKey" });
                }
                else
                {
                    WriteInformation($"Disconnected from {PowershellYKText.FriendlyName(YubiKeyModule._yubikey)} with serial: N/A.", new string[] { "YubiKey" });
                }

                // Clear YubiKey connection and credentials
                YubiKeyModule._yubikey = null;
                YubiKeyModule.clearPassword();
            }
        }
    }
}