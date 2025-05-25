/// <summary>
/// Utility class for Base32 encoding and decoding using the standard alphabet (A-Z2-7)
/// </summary>

using System;
using System.Text;

namespace powershellYK.support
{

    public static class Base32
    {
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const int BitsPerChar = 5;
        private const int BitsPerByte = 8;

        /// Encodes a byte array to a Base32 string
        public static string Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                return string.Empty;

            // Calculate the number of characters needed
            int charCount = (int)Math.Ceiling(data.Length * BitsPerByte / (double)BitsPerChar);
            var result = new StringBuilder(charCount);

            int buffer = 0;
            int bufferBits = 0;

            foreach (byte b in data)
            {
                buffer = (buffer << BitsPerByte) | b;
                bufferBits += BitsPerByte;

                while (bufferBits >= BitsPerChar)
                {
                    bufferBits -= BitsPerChar;
                    int index = (buffer >> bufferBits) & 0x1F;
                    result.Append(Base32Alphabet[index]);
                }
            }

            // Handle remaining bits
            if (bufferBits > 0)
            {
                buffer <<= (BitsPerChar - bufferBits);
                int index = buffer & 0x1F;
                result.Append(Base32Alphabet[index]);
            }

            return result.ToString();
        }

        /// Decodes a Base32 string to a byte array
        public static byte[] Decode(string base32String)
        {
            if (string.IsNullOrEmpty(base32String))
                return Array.Empty<byte>();

            // Remove any padding characters and convert to uppercase
            base32String = base32String.TrimEnd('=').ToUpper();

            // Calculate the number of bytes needed
            int byteCount = (int)Math.Floor(base32String.Length * BitsPerChar / (double)BitsPerByte);
            var result = new byte[byteCount];

            int buffer = 0;
            int bufferBits = 0;
            int byteIndex = 0;

            foreach (char c in base32String)
            {
                int value = Base32Alphabet.IndexOf(c);
                if (value == -1)
                    throw new ArgumentException($"Invalid Base32 character: {c}");

                buffer = (buffer << BitsPerChar) | value;
                bufferBits += BitsPerChar;

                while (bufferBits >= BitsPerByte)
                {
                    bufferBits -= BitsPerByte;
                    result[byteIndex++] = (byte)((buffer >> bufferBits) & 0xFF);
                }
            }

            return result;
        }
    }
} 