/// <summary>
/// Represents a FIDO2 credential with user and relying party information.
/// Provides access to credential details and user information.
/// 
/// .EXAMPLE
/// # Create a new credential
/// $credential = [powershellYK.FIDO2.Credential]::new($relyingParty, $credentialUserInfo)
/// Write-Host "Credential for user: $($credential.UserName)"
/// 
/// .EXAMPLE
/// # Get credential display information
/// Write-Host "Display Name: $($credential.DisplayName)"
/// Write-Host "Relying Party: $($credential.RPId)"
/// </summary>

// Imports
using Newtonsoft.Json.Linq;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.Cose;

namespace powershellYK.FIDO2
{
    // Represents a FIDO2 credential with user and relying party information
    public class Credential
    {
        // User's display name from credential info
        public string? DisplayName { get { return this.CredentialUserInfo.User.DisplayName; } }

        // Username associated with the credential
        public string? UserName { get { return this.CredentialUserInfo.User.Name; } }

        // Relying party identifier
        public string? RPId { get { return this.RelyingParty.Id; } }

        // Unique identifier for the credential
        public powershellYK.FIDO2.CredentialID CredentialID { get; private set; }

        // Relying party information (hidden from PowerShell)
        [Hidden]
        public RelyingParty RelyingParty { get; private set; }

        // User information associated with the credential (hidden from PowerShell)
        [Hidden]
        public CredentialUserInfo CredentialUserInfo { get; private set; }

        // Creates a new credential instance
        public Credential(RelyingParty relyingParty, CredentialUserInfo credentialUserInfo)
        {
            this.CredentialID = new powershellYK.FIDO2.CredentialID(credentialUserInfo.CredentialId);
            this.RelyingParty = relyingParty;
            this.CredentialUserInfo = credentialUserInfo;
        }

        #region Operators
        #endregion // Operators
    }
}
