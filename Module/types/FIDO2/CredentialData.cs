/// <summary>
/// Represents FIDO2 credential data for authentication operations.
/// Handles credential creation, encoding, and conversion between formats.
/// 
/// .EXAMPLE
/// # Create credential data
/// $credentialData = [powershellYK.FIDO2.CredentialData]::new($makeCredentialData, $clientData, $userEntity, $relyingParty)
/// Write-Host $credentialData.ToString()
/// 
/// .EXAMPLE
/// # Get base64url safe credential ID
/// $credentialId = $credentialData.GetBase64UrlSafeCredentialID()
/// Write-Host "Credential ID: $credentialId"
/// </summary>

// Imports
using powershellYK.support;
using System.Formats.Cbor;
using System.Runtime.CompilerServices;
using System.Text;
using Yubico.YubiKey.Fido2;

namespace powershellYK.FIDO2
{
    // Represents FIDO2 credential data for authentication operations
    public class CredentialData
    {
        // Properties for accessing credential data
        public MakeCredentialData MakeCredentialData { get { return this._makeCredentialData; } }
        public string ClientDataJSON { get { return this._clientDataJSON; } }

        // Internal storage for credential components
        private readonly string _clientDataJSON;
        private readonly MakeCredentialData _makeCredentialData;
        private readonly UserEntity _userEntity;
        private readonly RelyingParty _relyingParty;

        // Creates a new credential data instance
        public CredentialData(MakeCredentialData MakeCredentialData, string clientData, UserEntity userEntity, RelyingParty relyingParty)
        {
            this._makeCredentialData = MakeCredentialData;
            this._clientDataJSON = clientData;
            this._userEntity = userEntity;
            this._relyingParty = relyingParty;
        }

        // Returns a string representation of the credential
        public override string ToString()
        {
            var username = _userEntity.DisplayName ?? _userEntity.Name ?? "Unknown";
            var site = _relyingParty.Name ?? _relyingParty.Id ?? "Unknown";
            return $"Credential for {username} on {site}";
        }

        // Returns the client data JSON as a base64 string
        public string GetBase64clientDataJSON()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(this._clientDataJSON));
        }

        // Returns the attestation object as a base64 string
        public string GetBase64AttestationObject()
        {
            // Create CBOR writer for attestation object
            var writer = new CborWriter();
            writer.WriteStartMap(3);

            // Write format
            writer.WriteTextString("fmt");
            writer.WriteTextString(this._makeCredentialData.Format);

            // Write attestation statement
            writer.WriteTextString("attStmt");
            writer.WriteEncodedValue(_makeCredentialData.EncodedAttestationStatement.Span);

            // Write authenticator data
            writer.WriteTextString("authData");
            writer.WriteByteString(_makeCredentialData.AuthenticatorData.EncodedAuthenticatorData.Span);

            writer.WriteEndMap();
            var cborEncoded = writer.Encode();
            return Convert.ToBase64String(cborEncoded);
        }

        // Returns the credential ID as a base64url safe string
        public string GetBase64UrlSafeCredentialID()
        {
            // Return the CredentialID as a base64url string
            return Convert.ToBase64String(this._makeCredentialData.AuthenticatorData.CredentialId!.Id.ToArray()).Replace('+', '-').Replace('/', '_').Replace("=", "");
        }
    }
}
