using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using powershellYK.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Get, "YubiKey")]
    public class GetYubikeyCommand : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
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
        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(new YubikeyInformation(yubiKey: (YubiKeyDevice)YubiKeyDevice.FindAll().Where(yk => yk.SerialNumber == YubiKeyModule._yubikey!.SerialNumber).First()));
            }
            catch (System.InvalidOperationException e)
            {
                WriteWarning("No YubiKeys found, FIDO-only YubiKeys on Windows requires running as Administrator.");
                throw new Exception(e.Message, e);
            }
        }
    }
}