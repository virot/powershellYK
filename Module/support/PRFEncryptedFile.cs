/// <summary>
/// Represents the binary file format for FIDO2 PRF-encrypted files.
/// Handles serialization and deserialization of the encrypted file header
/// containing credential ID, relying party ID, salt, IV, authentication tag, and ciphertext.
///
/// .EXAMPLE
/// // Serialize an encrypted file
/// var encFile = new PRFEncryptedFile(credId, rpId, salt, iv, tag, ciphertext);
/// byte[] fileBytes = encFile.Serialize();
///
/// .EXAMPLE
/// // Deserialize an encrypted file
/// var encFile = PRFEncryptedFile.Deserialize(fileBytes);
/// byte[] salt = encFile.Salt;
/// </summary>

// Imports
using System.Buffers.Binary;
using System.Text;

namespace powershellYK.support
{
    // Binary file format handler for FIDO2 PRF-encrypted files
    public class PRFEncryptedFile
    {
        // File format magic bytes and version identifier
        private static readonly byte[] Magic = "YKPRF\x01\x00\x00"u8.ToArray();

        // Fixed-size field lengths
        public const int SaltLength = 32;
        public const int IVLength = 12;
        public const int TagLength = 16;

        // Parsed header and payload fields
        public byte[] CredentialId { get; }
        public string RelyingPartyId { get; }
        public byte[] Salt { get; }
        public byte[] IV { get; }
        public byte[] Tag { get; }
        public byte[] Ciphertext { get; }

        // Create a new encrypted file representation from individual components
        public PRFEncryptedFile(byte[] credentialId, string relyingPartyId, byte[] salt, byte[] iv, byte[] tag, byte[] ciphertext)
        {
            if (salt.Length != SaltLength)
                throw new ArgumentException($"Salt must be {SaltLength} bytes.", nameof(salt));
            if (iv.Length != IVLength)
                throw new ArgumentException($"IV must be {IVLength} bytes.", nameof(iv));
            if (tag.Length != TagLength)
                throw new ArgumentException($"Tag must be {TagLength} bytes.", nameof(tag));

            CredentialId = credentialId;
            RelyingPartyId = relyingPartyId;
            Salt = salt;
            IV = iv;
            Tag = tag;
            Ciphertext = ciphertext;
        }

        // Serialize the encrypted file to a byte array with structured header
        public byte[] Serialize()
        {
            byte[] rpIdBytes = Encoding.UTF8.GetBytes(RelyingPartyId);

            int totalLength = Magic.Length
                + 2 + CredentialId.Length
                + 2 + rpIdBytes.Length
                + SaltLength
                + IVLength
                + TagLength
                + Ciphertext.Length;

            byte[] output = new byte[totalLength];
            int offset = 0;

            // Write magic bytes
            Buffer.BlockCopy(Magic, 0, output, offset, Magic.Length);
            offset += Magic.Length;

            // Write credential ID (length-prefixed, big-endian)
            BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(offset), (ushort)CredentialId.Length);
            offset += 2;
            Buffer.BlockCopy(CredentialId, 0, output, offset, CredentialId.Length);
            offset += CredentialId.Length;

            // Write relying party ID (length-prefixed, big-endian, UTF-8)
            BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(offset), (ushort)rpIdBytes.Length);
            offset += 2;
            Buffer.BlockCopy(rpIdBytes, 0, output, offset, rpIdBytes.Length);
            offset += rpIdBytes.Length;

            // Write fixed-size cryptographic fields
            Buffer.BlockCopy(Salt, 0, output, offset, SaltLength);
            offset += SaltLength;

            Buffer.BlockCopy(IV, 0, output, offset, IVLength);
            offset += IVLength;

            Buffer.BlockCopy(Tag, 0, output, offset, TagLength);
            offset += TagLength;

            // Write ciphertext payload
            Buffer.BlockCopy(Ciphertext, 0, output, offset, Ciphertext.Length);

            return output;
        }

        // Deserialize a byte array back into a PRFEncryptedFile instance
        public static PRFEncryptedFile Deserialize(byte[] data)
        {
            int offset = 0;

            // Validate magic bytes
            if (data.Length < Magic.Length)
                throw new InvalidDataException("File is too small to be a valid PRF-encrypted file.");

            for (int i = 0; i < Magic.Length; i++)
            {
                if (data[i] != Magic[i])
                    throw new InvalidDataException("Invalid file magic. This is not a PRF-encrypted file.");
            }
            offset += Magic.Length;

            // Read credential ID (length-prefixed)
            if (data.Length < offset + 2)
                throw new InvalidDataException("Unexpected end of file reading credential ID length.");
            ushort credIdLen = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset));
            offset += 2;

            if (data.Length < offset + credIdLen)
                throw new InvalidDataException("Unexpected end of file reading credential ID.");
            byte[] credentialId = new byte[credIdLen];
            Buffer.BlockCopy(data, offset, credentialId, 0, credIdLen);
            offset += credIdLen;

            // Read relying party ID (length-prefixed, UTF-8)
            if (data.Length < offset + 2)
                throw new InvalidDataException("Unexpected end of file reading relying party ID length.");
            ushort rpIdLen = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset));
            offset += 2;

            if (data.Length < offset + rpIdLen)
                throw new InvalidDataException("Unexpected end of file reading relying party ID.");
            string relyingPartyId = Encoding.UTF8.GetString(data, offset, rpIdLen);
            offset += rpIdLen;

            // Read fixed-size cryptographic fields
            int remaining = SaltLength + IVLength + TagLength;
            if (data.Length < offset + remaining)
                throw new InvalidDataException("Unexpected end of file reading salt, IV, or tag.");

            byte[] salt = new byte[SaltLength];
            Buffer.BlockCopy(data, offset, salt, 0, SaltLength);
            offset += SaltLength;

            byte[] iv = new byte[IVLength];
            Buffer.BlockCopy(data, offset, iv, 0, IVLength);
            offset += IVLength;

            byte[] tag = new byte[TagLength];
            Buffer.BlockCopy(data, offset, tag, 0, TagLength);
            offset += TagLength;

            // Read remaining ciphertext payload
            int ciphertextLen = data.Length - offset;
            if (ciphertextLen < 0)
                throw new InvalidDataException("Unexpected end of file reading ciphertext.");

            byte[] ciphertext = new byte[ciphertextLen];
            if (ciphertextLen > 0)
                Buffer.BlockCopy(data, offset, ciphertext, 0, ciphertextLen);

            return new PRFEncryptedFile(credentialId, relyingPartyId, salt, iv, tag, ciphertext);
        }
    }
}
