using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.FIDO2;
using powershellYK.support;


namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubikeyFIDO2")]

    public class GetYubikeyFIDO2Cmdlet : Cmdlet
    {
        protected override void BeginProcessing()
        {
            {
                if (YubiKeyModule._yubikey is null)
                {
                    WriteDebug("No YubiKey selected, calling Connect-Yubikey");
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
            }
            if (Windows.IsRunningAsAdministrator() == false)
            {
                throw new Exception("FIDO access on Windows requires running as Administrator.");
            }
        }

        protected override void ProcessRecord()
        {
            using (var fido2Session = new Fido2Session((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                fido2Session.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                AuthenticatorInfo info = fido2Session.AuthenticatorInfo;
                WriteObject(new Information(info));

            }
        }
    }
}
