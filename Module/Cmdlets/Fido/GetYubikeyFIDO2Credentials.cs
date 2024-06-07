using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;
using Yubico.YubiKey.Fido2;
using powershellYK.support;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using powershellYK.FIDO2;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsCommon.Get, "YubikeyFIDO2Credentials")]

    public class GetYubikeyFIDO2CredentialsCommand : Cmdlet
    {
        protected override void BeginProcessing()
        {
            // If already connected disconnect first
            if (YubiKeyModule._fido2Session is null)
            {
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyFido2");
                myPowersShellInstance.Invoke();
            }
#if WINDOWS
                PermisionsStuff permisionsStuff = new PermisionsStuff();
                if (PermisionsStuff.IsRunningAsAdministrator() == false)
                {
                    throw new Exception("You need to run this command as an administrator");
                }
#endif //WINDOWS

            try
            {
                var RelyingParties = YubiKeyModule._fido2Session!.EnumerateRelyingParties();
                foreach (RelyingParty RelyingParty in RelyingParties)
                {
                    var relayCredentials = YubiKeyModule._fido2Session!.EnumerateCredentialsForRelyingParty(RelyingParty);
                    foreach (CredentialUserInfo user in relayCredentials)
                    {
                        Credentials credentials = new Credentials
                        {
                            Site = RelyingParty.Id,
                            Name = user.User.Name,
                            DisplayName = user.User.DisplayName,
                        };
                        WriteObject(credentials);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not EnumerateRelyingParties", e);
            }
        }
    }
}