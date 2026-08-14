/// <summary>
/// ARKG-P256 public-key derivation used by FIDO2 previewSign (firmware 5.8).
/// Matches the Yubico.Core test-helper construction: hash-to-scalar (RFC 9380
/// expand_message_xmd/SHA-256 reduced mod N), HMAC-KEM, then BL blinding.
/// </summary>

using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace powershellYK.support.FIDO2
{
    internal static class ArkgP256
    {
        private const int CoordinateLength = 32;
        private const int Sec1Length = 65;
        private const byte Sec1Tag = 0x04;
        private const string DstExt = "ARKG-P256";

        private static readonly BigInteger P = BigInteger.Parse(
            "00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF",
            NumberStyles.HexNumber,
            CultureInfo.InvariantCulture);

        private static readonly BigInteger N = BigInteger.Parse(
            "00FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551",
            NumberStyles.HexNumber,
            CultureInfo.InvariantCulture);

        private static readonly BigInteger A = P - 3;

        private static readonly BigInteger Gx = BigInteger.Parse(
            "006B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296",
            NumberStyles.HexNumber,
            CultureInfo.InvariantCulture);

        private static readonly BigInteger Gy = BigInteger.Parse(
            "004FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5",
            NumberStyles.HexNumber,
            CultureInfo.InvariantCulture);

        public static (byte[] derivedPublicKey, byte[] arkgKeyHandle) DerivePublicKey(
            ReadOnlySpan<byte> blindingPublicKey,
            ReadOnlySpan<byte> kemPublicKey,
            ReadOnlySpan<byte> inputKeyingMaterial,
            ReadOnlySpan<byte> context)
        {
            if (context.Length > 64)
            {
                throw new ArgumentException("ARKG context must be at most 64 bytes.", nameof(context));
            }

            byte[] contextPrime = new byte[1 + context.Length];
            contextPrime[0] = (byte)context.Length;
            context.CopyTo(contextPrime.AsSpan(1));

            byte[] contextKem = Concat(System.Text.Encoding.ASCII.GetBytes("ARKG-Derive-Key-KEM."), contextPrime);
            byte[] contextBl = Concat(System.Text.Encoding.ASCII.GetBytes("ARKG-Derive-Key-BL."), contextPrime);

            (byte[] ikmTau, byte[] cipher) = HmacKemEncaps(kemPublicKey, inputKeyingMaterial, contextKem);

            byte[] dstTau = Concat(
                System.Text.Encoding.ASCII.GetBytes("ARKG-BL-EC."),
                System.Text.Encoding.ASCII.GetBytes(DstExt),
                contextBl);
            BigInteger tau = HashToScalar(ikmTau, dstTau);
            byte[] derivedPublicKey = BlindPublicKey(blindingPublicKey, tau);
            return (derivedPublicKey, cipher);
        }

        private static (byte[] shared, byte[] ciphertext) HmacKemEncaps(
            ReadOnlySpan<byte> kemPublicKey,
            ReadOnlySpan<byte> inputKeyingMaterial,
            ReadOnlySpan<byte> context)
        {
            byte[] dstAug = System.Text.Encoding.ASCII.GetBytes("ARKG-ECDH.ARKG-P256");

            byte[] dstKg = Concat(
                System.Text.Encoding.ASCII.GetBytes("ARKG-KEM-ECDH-KG.ARKG-ECDH."),
                System.Text.Encoding.ASCII.GetBytes(DstExt));
            BigInteger ephemeralSecret = HashToScalar(inputKeyingMaterial, dstKg);
            byte[] ephemeralPublicKey = ScalarMulGenerator(ephemeralSecret);

            byte[] kPrime = EcdhX(ephemeralSecret, kemPublicKey);

            byte[] macInfo = Concat(System.Text.Encoding.ASCII.GetBytes("ARKG-KEM-HMAC-mac."), dstAug, context.ToArray());
            byte[] mk = HKDF.DeriveKey(HashAlgorithmName.SHA256, kPrime, outputLength: 32, salt: Array.Empty<byte>(), info: macInfo);

            byte[] fullMac = HMACSHA256.HashData(mk, ephemeralPublicKey);
            byte[] tag = fullMac.AsSpan(0, 16).ToArray();

            byte[] sharedInfo = Concat(System.Text.Encoding.ASCII.GetBytes("ARKG-KEM-HMAC-shared."), dstAug, context.ToArray());
            byte[] shared = HKDF.DeriveKey(HashAlgorithmName.SHA256, kPrime, outputLength: kPrime.Length, salt: Array.Empty<byte>(), info: sharedInfo);

            byte[] ciphertext = new byte[tag.Length + ephemeralPublicKey.Length];
            tag.CopyTo(ciphertext.AsSpan());
            ephemeralPublicKey.CopyTo(ciphertext.AsSpan(tag.Length));
            return (shared, ciphertext);
        }

        private static byte[] BlindPublicKey(ReadOnlySpan<byte> blindingPublicKey, BigInteger tau)
        {
            var (px, py) = ParseSec1(blindingPublicKey);
            var (qx, qy) = ScalarMul(tau, Gx, Gy);
            var (rx, ry) = PointAdd(px, py, qx, qy);
            return EncodeSec1(rx, ry);
        }

        private static byte[] ScalarMulGenerator(BigInteger scalar) => EncodeSec1(ScalarMul(scalar, Gx, Gy));

        private static byte[] EcdhX(BigInteger privateScalar, ReadOnlySpan<byte> publicPoint)
        {
            var (px, py) = ParseSec1(publicPoint);
            var (sx, _) = ScalarMul(privateScalar, px, py);
            return ToFixedWidthBytes(sx);
        }

        private static (BigInteger X, BigInteger Y) ParseSec1(ReadOnlySpan<byte> sec1)
        {
            if (sec1.Length != Sec1Length || sec1[0] != Sec1Tag)
            {
                throw new CryptographicException("Expected a 65-byte uncompressed P-256 point.");
            }

            return (Os2Ip(sec1.Slice(1, CoordinateLength)), Os2Ip(sec1.Slice(1 + CoordinateLength, CoordinateLength)));
        }

        private static byte[] EncodeSec1(BigInteger x, BigInteger y)
        {
            byte[] sec1 = new byte[Sec1Length];
            sec1[0] = Sec1Tag;
            ToFixedWidthBytes(x).CopyTo(sec1.AsSpan(1));
            ToFixedWidthBytes(y).CopyTo(sec1.AsSpan(1 + CoordinateLength));
            return sec1;
        }

        private static byte[] EncodeSec1((BigInteger X, BigInteger Y) point) => EncodeSec1(point.X, point.Y);

        private static (BigInteger X, BigInteger Y) ScalarMul(BigInteger k, BigInteger px, BigInteger py)
        {
            k %= N;
            if (k.Sign < 0)
            {
                k += N;
            }

            bool haveResult = false;
            BigInteger rx = BigInteger.Zero;
            BigInteger ry = BigInteger.Zero;
            BigInteger qx = px;
            BigInteger qy = py;

            for (int i = 0; i < 256; i++)
            {
                if (!k.IsZero && !k.IsEven)
                {
                    if (!haveResult)
                    {
                        rx = qx;
                        ry = qy;
                        haveResult = true;
                    }
                    else
                    {
                        (rx, ry) = PointAdd(rx, ry, qx, qy);
                    }
                }

                k >>= 1;
                if (k.IsZero)
                {
                    break;
                }

                (qx, qy) = PointDouble(qx, qy);
            }

            if (!haveResult)
            {
                throw new CryptographicException("ARKG scalar multiplication produced the point at infinity.");
            }

            return (rx, ry);
        }

        private static (BigInteger X, BigInteger Y) PointDouble(BigInteger x, BigInteger y)
        {
            BigInteger lambda = ModMul(ModAdd(ModMul(3, ModMul(x, x)), A), ModInverse(ModMul(2, y)));
            BigInteger x3 = ModSub(ModMul(lambda, lambda), ModMul(2, x));
            BigInteger y3 = ModSub(ModMul(lambda, ModSub(x, x3)), y);
            return (x3, y3);
        }

        private static (BigInteger X, BigInteger Y) PointAdd(BigInteger x1, BigInteger y1, BigInteger x2, BigInteger y2)
        {
            if (x1 == x2)
            {
                if (y1 == y2)
                {
                    return PointDouble(x1, y1);
                }

                throw new CryptographicException("ARKG point addition produced the point at infinity.");
            }

            BigInteger lambda = ModMul(ModSub(y2, y1), ModInverse(ModSub(x2, x1)));
            BigInteger x3 = ModSub(ModSub(ModMul(lambda, lambda), x1), x2);
            BigInteger y3 = ModSub(ModMul(lambda, ModSub(x1, x3)), y1);
            return (x3, y3);
        }

        private static BigInteger HashToScalar(ReadOnlySpan<byte> msg, ReadOnlySpan<byte> dst)
        {
            const int L = 48;
            byte[] uniform = ExpandMessageXmdSha256(msg, dst, L);
            return Os2Ip(uniform) % N;
        }

        private static byte[] ExpandMessageXmdSha256(ReadOnlySpan<byte> msg, ReadOnlySpan<byte> dst, int lenInBytes)
        {
            const int bInBytes = 32;
            const int sInBytes = 64;
            int ell = (lenInBytes + bInBytes - 1) / bInBytes;
            if (ell > 255 || lenInBytes > 65535 || dst.Length > 255)
            {
                throw new ArgumentException("expand_message_xmd parameter out of range.");
            }

            byte[] dstPrime = new byte[dst.Length + 1];
            dst.CopyTo(dstPrime);
            dstPrime[dst.Length] = (byte)dst.Length;

            byte[] msgPrime = Concat(new byte[sInBytes], msg.ToArray(), [(byte)((lenInBytes >> 8) & 0xFF), (byte)(lenInBytes & 0xFF)], [0x00], dstPrime);

            byte[][] bVals = new byte[ell + 1][];
            bVals[0] = SHA256.HashData(msgPrime);
            bVals[1] = SHA256.HashData(Concat(bVals[0], [0x01], dstPrime));

            byte[] xored = new byte[bInBytes];
            for (int i = 2; i <= ell; i++)
            {
                for (int j = 0; j < bInBytes; j++)
                {
                    xored[j] = (byte)(bVals[0][j] ^ bVals[i - 1][j]);
                }

                bVals[i] = SHA256.HashData(Concat(xored, [(byte)i], dstPrime));
            }

            byte[] result = new byte[lenInBytes];
            int offset = 0;
            for (int i = 1; i <= ell && offset < lenInBytes; i++)
            {
                int copy = Math.Min(bInBytes, lenInBytes - offset);
                bVals[i].AsSpan(0, copy).CopyTo(result.AsSpan(offset));
                offset += copy;
            }

            return result;
        }

        private static BigInteger Os2Ip(ReadOnlySpan<byte> bytes)
        {
            byte[] padded = new byte[bytes.Length + 1];
            for (int i = 0; i < bytes.Length; i++)
            {
                padded[bytes.Length - 1 - i] = bytes[i];
            }

            return new BigInteger(padded);
        }

        private static byte[] ToFixedWidthBytes(BigInteger value)
        {
            byte[] littleEndian = value.ToByteArray();
            int length = littleEndian.Length;
            if (length > 1 && littleEndian[length - 1] == 0)
            {
                length--;
            }

            byte[] bigEndian = new byte[CoordinateLength];
            int copyLength = Math.Min(length, CoordinateLength);
            for (int i = 0; i < copyLength; i++)
            {
                bigEndian[CoordinateLength - 1 - i] = littleEndian[i];
            }

            return bigEndian;
        }

        private static BigInteger ModAdd(BigInteger a, BigInteger b) => ((a + b) % P + P) % P;
        private static BigInteger ModSub(BigInteger a, BigInteger b) => ((a - b) % P + P) % P;
        private static BigInteger ModMul(BigInteger a, BigInteger b) => ((a * b) % P + P) % P;
        private static BigInteger ModInverse(BigInteger a) => BigInteger.ModPow((a % P + P) % P, P - 2, P);

        private static byte[] Concat(params byte[][] parts)
        {
            int length = 0;
            foreach (byte[] part in parts)
            {
                length += part.Length;
            }

            byte[] result = new byte[length];
            int offset = 0;
            foreach (byte[] part in parts)
            {
                part.CopyTo(result, offset);
                offset += part.Length;
            }

            return result;
        }
    }
}
