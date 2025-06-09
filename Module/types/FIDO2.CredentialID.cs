/// <summary>
/// Represents a FIDO2 credential identifier with conversion capabilities.
/// Handles credential ID creation and conversion between different formats.
/// 
/// .EXAMPLE
/// # Create credential ID from byte array
/// $credentialId = [powershellYK.FIDO2.CredentialID]::new($byteArray)
/// Write-Host $credentialId.ToString()
/// 
/// .EXAMPLE
/// # Create credential ID from base64url string
/// $credentialId = [powershellYK.FIDO2.CredentialID]::FromStringBase64URL($base64String)
/// $byteArray = $credentialId.ToByte()
/// </summary>

// Imports
using powershellYK.support;
using System.Formats.Cbor;
using System.Security.Cryptography;
using Yubico.YubiKey.Fido2;

namespace powershellYK.FIDO2
{
    // Represents a FIDO2 credential identifier with conversion capabilities
    public readonly struct CredentialID
    {
        // Internal YubiKey credential ID
        private readonly Yubico.YubiKey.Fido2.CredentialId _YCredentialId;

        // Creates a new credential ID from a YubiKey credential ID
        public CredentialID(Yubico.YubiKey.Fido2.CredentialId value)
        {
            _YCredentialId = value;
        }

        // Creates a new credential ID from a byte array
        public CredentialID(byte[] value)
        {
            var writer = new CborWriter(CborConformanceMode.Ctap2Canonical);
            writer.WriteStartMap(3);
            writer.WriteTextString("type");
            writer.WriteTextString("public-key");
            writer.WriteTextString("id");
            writer.WriteByteString(value);
            writer.WriteTextString("transports");
            writer.WriteStartArray(0);
            writer.WriteEndArray();
            writer.WriteEndMap();

            _YCredentialId = new Yubico.YubiKey.Fido2.CredentialId(writer.Encode(), out var bytesRead);
        }

        // Creates a new credential ID from a string
        public CredentialID(string value)
        {
            var writer = new CborWriter(CborConformanceMode.Ctap2Canonical);
            writer.WriteStartMap(3);
            writer.WriteTextString("type");
            writer.WriteTextString("public-key");
            writer.WriteTextString("id");
            writer.WriteByteString(Converter.StringToByteArray(value));
            writer.WriteTextString("transports");
            writer.WriteStartArray(0);
            writer.WriteEndArray();
            writer.WriteEndMap();

            _YCredentialId = new Yubico.YubiKey.Fido2.CredentialId(writer.Encode(), out var bytesRead);
        }

        // Creates a credential ID from a base64url string
        public static CredentialID FromStringBase64URL(string value)
        {
            return new CredentialID(System.Convert.FromBase64String(Converter.RemoveBase64URLSafe(Converter.AddMissingPadding(value))));
        }

        #region Destinations

        // Returns a string representation of the credential ID
        public override string ToString()
        {
            return Converter.ByteArrayToString(_YCredentialId.Id.ToArray()).ToLower();
        }

        // Returns the credential ID as a byte array
        public byte[] ToByte()
        {
            return _YCredentialId.Id.ToArray();
        }

        // Returns the YubiKey FIDO2 credential ID
        public Yubico.YubiKey.Fido2.CredentialId ToYubicoFIDO2CredentialID()
        {
            return _YCredentialId;
        }

        #endregion // Destinations

        #region Operators

        // Implicit conversion from YubiKey credential ID
        public static implicit operator CredentialID(Yubico.YubiKey.Fido2.CredentialId credentialID)
        {
            return new CredentialID(credentialID);
        }

        // Implicit conversion to YubiKey credential ID
        public static implicit operator Yubico.YubiKey.Fido2.CredentialId(CredentialID credentialID)
        {
            return credentialID.ToYubicoFIDO2CredentialID();
        }

        // Implicit conversion to ReadOnlyMemory<byte>
        public static implicit operator ReadOnlyMemory<byte>(powershellYK.FIDO2.CredentialID credentialID)
        {
            return credentialID.ToByte().AsMemory();
        }

        // Implicit conversion from string
        public static implicit operator CredentialID(string value)
        {
            byte[] credentialID = Converter.StringToByteArray(value);
            return new CredentialID(credentialID);
        }

        #endregion // Operators
    }
}
