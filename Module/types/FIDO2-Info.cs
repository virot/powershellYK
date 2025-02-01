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
    public class Information
    {
        [Hidden]
        public AuthenticatorInfo AuthenticatorInfo { get; }
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
        public IReadOnlyList<string> Versions { get { return AuthenticatorInfo.Versions; } }
        public IReadOnlyList<string>? Extensions { get { return AuthenticatorInfo.Extensions; } }
        public IReadOnlyDictionary<string, bool>? Options { get { return AuthenticatorInfo.Options; } }
        public int? MaximumMessageSize { get { return AuthenticatorInfo.MaximumMessageSize; } }
        public IReadOnlyList<PinUvAuthProtocol>? PinUvAuthProtocols { get { return AuthenticatorInfo.PinUvAuthProtocols; } }
        public int? MaximumCredentialCountInList { get { return AuthenticatorInfo.MaximumCredentialCountInList; } }
        public int? MaximumCredentialIdLength { get { return AuthenticatorInfo.MaximumCredentialIdLength; } }
        public IReadOnlyList<string>? Transports { get { return AuthenticatorInfo.Transports; } }
        public IReadOnlyList<Tuple<string, CoseAlgorithmIdentifier>>? Algorithms { get { return AuthenticatorInfo.Algorithms; } }
        public int? MaximumSerializedLargeBlobArray { get { return AuthenticatorInfo.MaximumSerializedLargeBlobArray; } }
        public bool? ForcePinChange { get { return AuthenticatorInfo.ForcePinChange; } }
        public int? MinimumPinLength { get { return AuthenticatorInfo.MinimumPinLength; } }
        public Version? FirmwareVersion;
        public int? MaximumCredentialBlobLength { get { return AuthenticatorInfo.MaximumCredentialBlobLength; } }
        public int? MaximumRpidsForSetMinPinLength { get { return AuthenticatorInfo.MaximumRpidsForSetMinPinLength; } }
        public int? PreferredPlatformUvAttempts { get { return AuthenticatorInfo.PreferredPlatformUvAttempts; } }
        public int? UvModality { get { return AuthenticatorInfo.UvModality; } }
        public IReadOnlyDictionary<string, int>? Certifications { get { return AuthenticatorInfo.Certifications; } }
        public int? RemainingDiscoverableCredentials { get { return AuthenticatorInfo.RemainingDiscoverableCredentials; } }
        public IReadOnlyList<long>? VendorPrototypeConfigCommands { get { return AuthenticatorInfo.VendorPrototypeConfigCommands; } }

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
