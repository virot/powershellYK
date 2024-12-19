using System.Management.Automation;
using Microsoft.Extensions.Logging;
using Yubico.Core.Logging;

namespace powershellYK.Cmdlets.Other
{
    [Cmdlet(VerbsLifecycle.Enable, "powershellYKSDKLogging")]
    public class Enable_PowershellYKSDKLoggingCmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            Log.ConfigureLoggerFactory(builder => builder.AddConsole());
        }
    }
}
