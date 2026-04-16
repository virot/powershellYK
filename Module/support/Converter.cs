/// <summary>
/// Utility class for converting between different data formats used in YubiKey operations.
/// Provides methods for hex string conversion, byte array manipulation, and key format conversion.
/// 
/// .EXAMPLE
/// $hexString = "1A2B3C4D"
/// $bytes = [powershellYK.support.Converter]::StringToByteArray($hexString)
/// 
/// .EXAMPLE
/// $bytes = [byte[]]@(0x1A, 0x2B, 0x3C, 0x4D)
/// $hexString = [powershellYK.support.Converter]::ByteArrayToString($bytes)
/// 
/// .EXAMPLE
/// $uid = [powershellYK.support.Converter]::SerialToNfcUid(12345678)
/// $hex = [powershellYK.support.Converter]::ByteArrayToString($uid)
/// Returns NFC ID in hex: "274E61BC00614E"
/// 
/// .EXAMPLE
/// $uid = [powershellYK.support.Converter]::SerialToNfcUid(12345678)
/// $dec = [powershellYK.support.Converter]::NfcUidToDecimal($uid)
/// Returns NFC ID in decimal: 11161651036004942
/// 
/// </summary>

// Imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yubico.YubiKey.Cryptography;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace powershellYK.support
{
    // Utility class for data format conversion
    public class Converter
    {
        // Convert hex string to byte array
        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        // Convert byte array to hex string
        public static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        // Swap adjacent bytes in a byte array
        public static void ByteSwapPairs(byte[] byteArray)
        {
            // Iterate through the array in steps of 2
            for (int i = 0; i < byteArray.Length; i += 2)
            {
                if (i + 1 < byteArray.Length)
                {
                    // Swap pairs in place
                    byte temp = byteArray[i];
                    byteArray[i] = byteArray[i + 1];
                    byteArray[i + 1] = temp;
                }
                // If there's an odd number of bytes, the last byte remains unchanged
            }
        }

        // Convert integer to Version object
        public static Version IntToVersion(int version)
        {
            byte[] versionBytes = new byte[] { (byte)(version >> 16), (byte)(version >> 8), (byte)version };
            return ByteArrayToVersion(versionBytes);
        }

        // Convert byte array to Version object
        public static Version ByteArrayToVersion(byte[] bytes)
        {
            if (bytes.Length != 3)
            {
                throw new ArgumentException("Version must be 3 bytes long");
            }
            return new Version(bytes[0], bytes[1], bytes[2]);
        }

        // Convert URL-safe base64 to standard base64
        internal static string RemoveBase64URLSafe(string urlsafe)
        {
            return urlsafe.Replace('-', '+').Replace('_', '/');
        }

        // Add padding to base64 string if needed
        internal static string AddMissingPadding(string base64)
        {
            // Calculate the number of padding characters needed
            int paddingCount = 4 - (base64.Length % 4);
            if (paddingCount != 4)
            {
                base64 += new string('=', paddingCount);
            }
            return base64;
        }

        // Derive the 7-byte NFC UID from a YubiKey serial number
        public static byte[] SerialToNfcUid(uint serial)
        {
            byte b0 = (byte)(serial >> 24);
            byte b1 = (byte)(serial >> 16);
            byte b2 = (byte)(serial >> 8);
            byte b3 = (byte)serial;

            return new byte[] { 0x27, b3, b2, b1, b0, b2, b3 };
        }

        // Derive the decimal representation of a 7-byte NFC UID
        public static ulong NfcUidToDecimal(byte[] uid)
        {
            if (uid.Length != 7)
            {
                throw new ArgumentException("NFC UID must be 7 bytes long", nameof(uid));
            }

            ulong value = 0;
            for (int i = 0; i < uid.Length; i++)
            {
                value = (value << 8) | uid[i];
            }
            return value;
        }

        // Convert YubiKey public key to .NET asymmetric algorithm
        public static AsymmetricAlgorithm YubiKeyPublicKeyToDotNet(IPublicKey publicKey)
        {
            // Handle RSA public key
            if (publicKey is RSAPublicKey rsaPublicKey)
            {
                RSA rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(publicKey.ExportSubjectPublicKeyInfo(), out _);
                return rsa;
            }
            // Handle ECC public key
            else if (publicKey is ECPublicKey eccPublicKey)
            {
                ECDsa ecc = ECDsa.Create();
                ecc.ImportSubjectPublicKeyInfo(publicKey.ExportSubjectPublicKeyInfo(), out _);
                return ecc;
            }
            // Handle Curve25519 public key
            else if (publicKey is Curve25519PublicKey curve25519PublicKey)
            {
                throw new NotImplementedException("Curve25519PublicKey is not implemented yet.");
            }
            else
            {
                throw new NotSupportedException("Unsupported public key type");
            }
        }
    }
}
