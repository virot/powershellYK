/// <summary>
/// Retrieves version information about the PowerShell YubiKey module and its dependencies.
/// Returns version numbers for the YubiKey SDK, PowerShell automation, and the module itself.
/// 
/// .EXAMPLE
/// Get-PowershellYKInfo
/// Returns version information for the PowerShell YubiKey module and its dependencies
/// </summary>

// Imports
using System;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsCommon.Get, "powershellYKInfo")]
    public class Get_PowershellYKInfo : Cmdlet
    {
        // Process version information
        protected override void BeginProcessing()
        {
            // Get all loaded assemblies
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (loadedAssemblies is not null)
            {
                // Find YubiKey SDK assembly
                Assembly? yubicoAssembly = loadedAssemblies.Where(assembly => 
                    assembly.GetName().Name!.Equals("Yubico.YubiKey", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                // Find PowerShell automation assembly
                Assembly? automationAssembly = loadedAssemblies.Where(assembly => 
                    assembly.GetName().Name!.Equals("System.Management.Automation", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                // Create and return version information object
                WriteObject(new powershellYKInfo(
                    yubicoAssembly is not null ? yubicoAssembly.GetName().Version : null,
                    automationAssembly is not null ? automationAssembly.GetName().Version : null,
                    Assembly.GetExecutingAssembly().GetName().Version
                ));
            }
        }
    }
}
