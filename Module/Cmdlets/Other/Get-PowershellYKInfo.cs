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
        protected override void BeginProcessing()
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (loadedAssemblies is not null)
            {
                Assembly? yubicoAssembly = loadedAssemblies.Where(assembly => assembly.GetName().Name.Equals("Yubico.YubiKey", StringComparison.OrdinalIgnoreCase)).FirstOrDefault(); // Use FirstOrDefault to get the first matching assembly or null if not found.
                Assembly? automationAssembly = loadedAssemblies.Where(assembly => assembly.GetName().Name.Equals("System.Management.Automation", StringComparison.OrdinalIgnoreCase)).FirstOrDefault(); // Use FirstOrDefault to get the first matching assembly or null if not found.

                WriteObject(new powershellYKInfo(
                    yubicoAssembly is not null ? yubicoAssembly.GetName().Version : null,
                    automationAssembly is not null ? automationAssembly.GetName().Version : null,
                    Assembly.GetExecutingAssembly().GetName().Version
                ));
            }
        }
    }
}
