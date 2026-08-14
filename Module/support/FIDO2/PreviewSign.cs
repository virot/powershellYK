/// <summary>
/// Local constants and ARKG-P256 helpers for the FIDO2 previewSign extension.
///
/// The Yubico .NET SDK ships on-device previewSign (MakeCredential / GetAssertion)
/// in Yubico.YubiKey, and the ARKG-P256 primitives in Yubico.Core. The COSE
/// algorithm identifier -65539 is not an SDK enum member, so it is redefined here.
/// ARKG derivation (seed public key -> derived P-256 key + signing ticket) is
/// required before GetAssertion; firmware 5.8 rejects a sign request without it.
///
/// .EXAMPLE
/// PreviewSign.FormatAlgorithm(PreviewSign.ArkgP256ESP256)
/// Returns "ArkgP256ESP256".
///
/// .EXAMPLE
/// PreviewSign.ParseAlgorithm("ArkgP256ESP256")
/// Returns the local -65539 COSE algorithm identifier.
/// </summary>

// Imports
using System.Formats.Cbor;
using System.Security.Cryptography;
using Yubico.YubiKey.Fido2.Cose;

namespace powershellYK.support.FIDO2
{
    // previewSign helper constants and ARKG-P256 derivation
    public static class PreviewSign
    {
        // ARKG-P256 / ESP256-split algorithm used by the YubiKey previewSign extension.
        // Defined locally because the SDK exposes it only in Yubico.YubiKey.TestUtilities.
        public const CoseAlgorithmIdentifier ArkgP256ESP256 = (CoseAlgorithmIdentifier)(-65539);

        // COSE key type and algorithm for the ARKG-P256 seed public key returned by firmware 5.8.
        private const int CoseKeyTypeArkgPub = -65537;
        private const int CoseAlgorithmArkgP256 = -65700;

        // Default domain-separation context used when the caller does not supply one.
        public const string DefaultDerivationContext = "powershellYK-previewsign";

        // Display name for a COSE algorithm identifier. Named SDK members keep their
        // enum name; previewSign values missing from the public enum are translated here.
        public static string FormatAlgorithm(CoseAlgorithmIdentifier algorithm)
        {
            if (algorithm == ArkgP256ESP256)
            {
                return nameof(ArkgP256ESP256);
            }
            if ((int)algorithm == CoseAlgorithmArkgP256)
            {
                return "ArkgP256";
            }
            return Enum.GetName(typeof(CoseAlgorithmIdentifier), algorithm) ?? ((int)algorithm).ToString();
        }

        // Parse a FormatAlgorithm name, SDK enum name, or numeric identifier
        public static CoseAlgorithmIdentifier ParseAlgorithm(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ArkgP256ESP256;
            }
            if (string.Equals(name, nameof(ArkgP256ESP256), StringComparison.OrdinalIgnoreCase))
            {
                return ArkgP256ESP256;
            }
            if (string.Equals(name, "ArkgP256", StringComparison.OrdinalIgnoreCase))
            {
                return (CoseAlgorithmIdentifier)CoseAlgorithmArkgP256;
            }
            if (int.TryParse(name, out int numeric))
            {
                return (CoseAlgorithmIdentifier)numeric;
            }
            if (Enum.TryParse(name, ignoreCase: true, out CoseAlgorithmIdentifier parsed))
            {
                return parsed;
            }
            throw new ArgumentException($"Unknown COSE algorithm '{name}'.");
        }

        // Result of one ARKG-P256 derivation from a previewSign seed public key.
        public sealed class DerivedSigningKey
        {
            public DerivedSigningKey(byte[] derivedPublicKeySec1, byte[] additionalArgs, byte[] context)
            {
                DerivedPublicKeySec1 = derivedPublicKeySec1;
                AdditionalArgs = additionalArgs;
                Context = context;
            }

            // Uncompressed SEC1 P-256 public key (0x04 || X || Y) used to verify the signature.
            public byte[] DerivedPublicKeySec1 { get; }

            // CBOR-encoded COSE_Sign_Args ticket passed as GetAssertion additionalArgs.
            public byte[] AdditionalArgs { get; }

            // Context bytes used for this derivation.
            public byte[] Context { get; }
        }

        // Derive a unique signing key and GetAssertion ticket from a previewSign seed public key.
        public static DerivedSigningKey DeriveSigningKey(ReadOnlyMemory<byte> seedPublicKeyCose, string? context = null)
        {
            byte[] ikm = RandomNumberGenerator.GetBytes(32);
            byte[] ctx = System.Text.Encoding.UTF8.GetBytes(string.IsNullOrWhiteSpace(context) ? DefaultDerivationContext : context);
            return DeriveSigningKey(seedPublicKeyCose, ikm, ctx);
        }

        public static DerivedSigningKey DeriveSigningKey(ReadOnlyMemory<byte> seedPublicKeyCose, ReadOnlySpan<byte> inputKeyingMaterial, ReadOnlySpan<byte> context)
        {
            var (blindingPublicKey, kemPublicKey) = ParseArkgCoseKey(seedPublicKeyCose.ToArray());
            var (derivedPublicKey, arkgKeyHandle) = ArkgP256.DerivePublicKey(
                blindingPublicKey,
                kemPublicKey,
                inputKeyingMaterial,
                context);

            byte[] additionalArgs = EncodeArkgSignArgs(arkgKeyHandle, context);
            return new DerivedSigningKey(derivedPublicKey, additionalArgs, context.ToArray());
        }

        // Encode COSE_Sign_Args: {3: -65539, -1: arkgKeyHandle, -2: context}
        private static byte[] EncodeArkgSignArgs(ReadOnlyMemory<byte> arkgKeyHandle, ReadOnlySpan<byte> context)
        {
            var cbor = new CborWriter(CborConformanceMode.Ctap2Canonical, convertIndefiniteLengthEncodings: true);
            cbor.WriteStartMap(3);
            cbor.WriteInt32(3);
            cbor.WriteInt32((int)ArkgP256ESP256);
            cbor.WriteInt32(-1);
            cbor.WriteByteString(arkgKeyHandle.Span);
            cbor.WriteInt32(-2);
            cbor.WriteByteString(context);
            cbor.WriteEndMap();
            return cbor.Encode();
        }

        private static (byte[] blindingPublicKey, byte[] kemPublicKey) ParseArkgCoseKey(byte[] coseEncoded)
        {
            var reader = new CborReader(coseEncoded, CborConformanceMode.Ctap2Canonical);
            int? entries = reader.ReadStartMap();
            int count = entries ?? int.MaxValue;

            bool? isArkgPubKey = null;
            bool? isArkgP256Key = null;
            byte[]? blindingPublicKey = null;
            byte[]? kemPublicKey = null;

            for (int i = 0; i < count; i++)
            {
                if (reader.PeekState() == CborReaderState.EndMap)
                {
                    break;
                }

                long key = reader.ReadInt64();
                if (key == 1)
                {
                    isArkgPubKey = reader.ReadInt32() == CoseKeyTypeArkgPub;
                }
                else if (key == 3)
                {
                    isArkgP256Key = reader.ReadInt32() == CoseAlgorithmArkgP256;
                }
                else if (key == -1)
                {
                    blindingPublicKey = ReadEc2PointAsSec1(reader);
                }
                else if (key == -2)
                {
                    kemPublicKey = ReadEc2PointAsSec1(reader);
                }
                else
                {
                    reader.SkipValue();
                }
            }

            reader.ReadEndMap();

            if (isArkgPubKey != true || isArkgP256Key != true)
            {
                throw new InvalidOperationException("previewSign seed public key is not an ARKG-P256 COSE key.");
            }

            if (blindingPublicKey is null || kemPublicKey is null)
            {
                throw new InvalidOperationException("previewSign seed public key is missing blinding (-1) or KEM (-2) public keys.");
            }

            return (blindingPublicKey, kemPublicKey);
        }

        private static byte[] ReadEc2PointAsSec1(CborReader reader)
        {
            int? subEntries = reader.ReadStartMap();
            int subCount = subEntries ?? int.MaxValue;

            bool? isEc2Key = null;
            bool? isP256Curve = null;
            byte[]? x = null;
            byte[]? y = null;
            for (int j = 0; j < subCount; j++)
            {
                if (reader.PeekState() == CborReaderState.EndMap)
                {
                    break;
                }

                long subKey = reader.ReadInt64();
                if (subKey == 1)
                {
                    isEc2Key = reader.ReadInt32() == (int)CoseKeyType.Ec2;
                }
                else if (subKey == 3)
                {
                    reader.SkipValue();
                }
                else if (subKey == -1)
                {
                    isP256Curve = reader.ReadInt32() == (int)CoseEcCurve.P256;
                }
                else if (subKey == -2)
                {
                    x = reader.ReadByteString();
                }
                else if (subKey == -3)
                {
                    y = reader.ReadByteString();
                }
                else
                {
                    reader.SkipValue();
                }
            }

            reader.ReadEndMap();

            if (isEc2Key != true || isP256Curve != true)
            {
                throw new InvalidOperationException("previewSign ARKG public-key components must be EC2 P-256 keys.");
            }

            if (x is null || y is null || x.Length != 32 || y.Length != 32)
            {
                throw new InvalidOperationException("previewSign EC2 point coordinates must be 32 bytes each.");
            }

            byte[] sec1 = new byte[65];
            sec1[0] = 0x04;
            Buffer.BlockCopy(x, 0, sec1, 1, 32);
            Buffer.BlockCopy(y, 0, sec1, 33, 32);
            return sec1;
        }
    }
}
