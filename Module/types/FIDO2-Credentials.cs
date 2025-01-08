using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace powershellYK.FIDO2
{
    public class Credential
    {
        public string? DisplayName { get; private set; }
        public string? UserName { get; private set; }
        public string? RPId { get; private set; }
        public string? CredID { get
            {
                byte[] credentialIdBytes = CredentialID.Id.ToArray();

                string credentialIdBase64 = Convert.ToBase64String(credentialIdBytes); 
                return credentialIdBase64;
            }
        }
        [Hidden]
        public CredentialId CredentialID { get; private set; }
        [Hidden]
        public CoseKey? coseKey { get; set; }

        public Credential(string RPId, string? UserName, string? DisplayName, CredentialId CredentialID)
        {
            this.RPId = RPId;
            this.UserName = UserName;
            this.DisplayName = DisplayName;
            this.CredentialID = CredentialID;
        }
    }
}
    