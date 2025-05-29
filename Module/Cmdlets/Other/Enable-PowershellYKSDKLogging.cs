/// <summary>
/// Enables logging for the PowerShell YubiKey SDK.
/// Configures console logging for debugging and troubleshooting purposes.
/// 
/// .EXAMPLE
/// Enable-PowershellYKSDKLogging
/// Enables console logging for the PowerShell YubiKey SDK
/// </summary>

// Imports
using System.Management.Automation;
using Microsoft.Extensions.Logging;
using Yubico.Core.Logging;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsLifecycle.Enable, "powershellYKSDKLogging")]
    public class Enable_PowershellYKSDKLoggingCmdlet : Cmdlet
    {
        // Initialize logging configuration
        protected override void BeginProcessing()
        {
            // Configure console logging
            Log.ConfigureLoggerFactory(builder => builder.AddConsole());
        }
    }
}
