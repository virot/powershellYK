/// <summary>
/// Represents version information for the PowerShell YubiKey module and its dependencies.
/// Provides access to version numbers for Yubico SDK, Automation, and the module itself.
/// 
/// .EXAMPLE
/// # Get version information
/// $info = [powershellYK.powershellYKInfo]::new(
///     $yubicoVersion,
///     $automationVersion,
///     $moduleVersion
/// )
/// Write-Host "Yubico SDK: $($info.YubicoVersion)"
/// 
/// .EXAMPLE
/// # Check module version
/// Write-Host "Module Version: $($info.powershellYKVersion)"
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;

namespace powershellYK
{
    // Represents version information for the PowerShell YubiKey module
    public class powershellYKInfo
    {
        // Yubico SDK version
        public Version? YubicoVersion { get; }

        // Automation version
        public Version? AutomationVersion { get; }

        // Module version
        public Version? powershellYKVersion { get; }

        // Creates a new version information instance
        public powershellYKInfo(
            Version? YubicoVersion,
            Version? AutomationVersion,
            Version? powershellYKVersion)
        {
            this.YubicoVersion = YubicoVersion;
            this.AutomationVersion = AutomationVersion;
            this.powershellYKVersion = powershellYKVersion;
        }
    }
}
