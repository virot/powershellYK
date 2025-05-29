/// <summary>
/// Represents FIDO2 authenticator information and capabilities.
/// Provides access to device capabilities, supported features, and configuration.
/// 
/// .EXAMPLE
/// # Get authenticator information
/// $info = [powershellYK.FIDO2.Information]::new($authenticatorInfo)
/// Write-Host "Firmware Version: $($info.FirmwareVersion)"
/// Write-Host "Supported Versions: $($info.Versions -join ', ')"
/// 
/// .EXAMPLE
/// # Check device capabilities
/// if ($info.Options['up']) {
///     Write-Host "Device supports user presence"
/// }
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey.Fido2;
using Yubico.YubiKey.Fido2.PinProtocols;
using Yubico.YubiKey.Fido2.Cose;
using powershellYK.support;

namespace powershellYK.FIDO2
{
    // Represents FIDO2 authenticator information and capabilities
    public class Information
    {
        // Internal authenticator information (hidden from PowerShell)
        [Hidden]
        public AuthenticatorInfo AuthenticatorInfo { get; }

        // Authenticator Attestation GUID
        public Guid? AAGuid
        {
            get
            {
                if (AuthenticatorInfo.Aaguid.Length != 16)
                {
                    return null;
                }
                byte[] tempArray = AuthenticatorInfo.Aaguid.ToArray();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(tempArray, 0, 4);
                    Array.Reverse(tempArray, 4, 2);
                    Array.Reverse(tempArray, 6, 2);
                }
                return new Guid(tempArray);
            }
        }

        // Supported FIDO2 versions
        public IReadOnlyList<string> Versions { get { return AuthenticatorInfo.Versions; } }

        // Supported extensions
        public IReadOnlyList<string>? Extensions { get { return AuthenticatorInfo.Extensions; } }

        // Device options and capabilities
        public IReadOnlyDictionary<string, bool>? Options { get { return AuthenticatorInfo.Options; } }

        // Maximum message size supported
        public int? MaximumMessageSize { get { return AuthenticatorInfo.MaximumMessageSize; } }

        // Supported PIN/UV authentication protocols
        public IReadOnlyList<PinUvAuthProtocol>? PinUvAuthProtocols { get { return AuthenticatorInfo.PinUvAuthProtocols; } }

        // Maximum number of credentials in list
        public int? MaximumCredentialCountInList { get { return AuthenticatorInfo.MaximumCredentialCountInList; } }

        // Maximum credential ID length
        public int? MaximumCredentialIdLength { get { return AuthenticatorInfo.MaximumCredentialIdLength; } }

        // Supported transport protocols
        public IReadOnlyList<string>? Transports { get { return AuthenticatorInfo.Transports; } }

        // Supported algorithms
        public IReadOnlyList<Tuple<string, CoseAlgorithmIdentifier>>? Algorithms { get { return AuthenticatorInfo.Algorithms; } }

        // Maximum serialized large blob array size
        public int? MaximumSerializedLargeBlobArray { get { return AuthenticatorInfo.MaximumSerializedLargeBlobArray; } }

        // Whether PIN change is required
        public bool? ForcePinChange { get { return AuthenticatorInfo.ForcePinChange; } }

        // Minimum PIN length required
        public int? MinimumPinLength { get { return AuthenticatorInfo.MinimumPinLength; } }

        // Device firmware version
        public Version? FirmwareVersion;

        // Maximum credential blob length
        public int? MaximumCredentialBlobLength { get { return AuthenticatorInfo.MaximumCredentialBlobLength; } }

        // Maximum RPIDs for minimum PIN length
        public int? MaximumRpidsForSetMinPinLength { get { return AuthenticatorInfo.MaximumRpidsForSetMinPinLength; } }

        // Preferred platform UV attempts
        public int? PreferredPlatformUvAttempts { get { return AuthenticatorInfo.PreferredPlatformUvAttempts; } }

        // UV modality
        public int? UvModality { get { return AuthenticatorInfo.UvModality; } }

        // Device certifications
        public IReadOnlyDictionary<string, int>? Certifications { get { return AuthenticatorInfo.Certifications; } }

        // Remaining discoverable credentials
        public int? RemainingDiscoverableCredentials { get { return AuthenticatorInfo.RemainingDiscoverableCredentials; } }

        // Vendor prototype configuration commands
        public IReadOnlyList<long>? VendorPrototypeConfigCommands { get { return AuthenticatorInfo.VendorPrototypeConfigCommands; } }

        // Creates a new instance of authenticator information
        public Information(AuthenticatorInfo authenticatorInfo)
        {
            this.AuthenticatorInfo = authenticatorInfo;
            if (authenticatorInfo.FirmwareVersion is not null)
            {
                this.FirmwareVersion = Converter.IntToVersion((int)authenticatorInfo.FirmwareVersion);
            }
        }
    }
}
