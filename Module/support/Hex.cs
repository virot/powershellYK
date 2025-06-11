/// <summary>
/// Utility class for hex encoding and decoding.
/// Provides methods to convert between byte arrays and hex strings.
/// 
/// .EXAMPLE
/// # Encode a byte array to hex
/// $bytes = [byte[]]@(1,2,3,4,5)
/// $hex = [powershellYK.support.Hex]::Encode($bytes)
/// 
/// .EXAMPLE
/// # Decode a hex string to bytes
/// $hex = "0102030405"
/// $bytes = [powershellYK.support.Hex]::Decode($hex)
/// </summary>

using System;
using System.Linq;

namespace powershellYK.support
{
    public static class Hex
    {
        // Encodes a byte array to a hex string
        public static string Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                return string.Empty;

            return string.Concat(data.Select(b => b.ToString("x2")));
        }

        // Decodes a hex string to a byte array
        public static byte[] Decode(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return Array.Empty<byte>();

            // Remove any whitespace and convert to lowercase
            hexString = hexString.Replace(" ", "").ToLower();

            // Ensure the string has an even length
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length");

            // Convert each pair of hex characters to a byte
            byte[] result = new byte[hexString.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                string hexPair = hexString.Substring(i * 2, 2);
                result[i] = Convert.ToByte(hexPair, 16);
            }

            return result;
        }
    }
} 