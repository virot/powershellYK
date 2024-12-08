using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubico.YubiKey.Piv.Objects;
using powershellYK.support;
using System.Security;
using System.Runtime.InteropServices;
using powershellYK.support.transform;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsSecurity.Unblock, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class UnblockYubikeyPIVCmdlet : Cmdlet
    {

        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        public SecureString NewPIN { get; set; } = new SecureString();


        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        public SecureString PUK { get; set; } = new SecureString();

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        protected override void ProcessRecord()
        {
            int? retriesLeft = null;
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                try
                {
                    if (pivSession.TryResetPin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!)
, System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN))!)
, out retriesLeft) == false)
                    {
                        throw new Exception("Incorrect PUK provided");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to reset PIN", e);
                }
                finally
                {
                }
            }
        }
    }
}
