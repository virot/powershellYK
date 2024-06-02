using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using VirotYubikey.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Oath;

namespace VirotYubikey.Cmdlets.OATH
{
    [Cmdlet(VerbsCommon.Get, "YubikeyOATHCredential")]

    public class GetYubikeyOATH2CredentialCommand : Cmdlet
    {

        protected override void ProcessRecord()
        {
            // If already connected disconnect first
            if (YubiKeyModule._oathSession is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyOath");
                myPowersShellInstance.Invoke();
            }
            try
            {
                IList<Credential> credentials = YubiKeyModule._oathSession!.GetCredentials();
                foreach (Credential credential in credentials)
                {
                    WriteObject(credential);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not enumerate credentials", e);
            }
        }
    }
}