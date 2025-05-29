/// <summary>
/// Generates SSH key identifiers from RSA and ECDSA public keys.
/// Formats keys according to SSH protocol specifications.
/// 
/// .EXAMPLE
/// $rsa = [System.Security.Cryptography.RSA]::Create()
/// $identifier = [powershellYK.support.GenerateIdentifier]::SSHIdentifier($rsa, "user@host")
/// 
/// .EXAMPLE
/// $ecdsa = [System.Security.Cryptography.ECDsa]::Create()
/// $identifier = [powershellYK.support.GenerateIdentifier]::SSHIdentifier($ecdsa, "user@host")
/// </summary>

// Imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace powershellYK.support
{
    // Utility class for generating SSH key identifiers
    internal class GenerateIdentifier
    {
        // Storage for curve bytes in ECDSA key generation
        private static byte[]? curveBytes;

        // Generate SSH identifier from RSA public key
        public static string SSHIdentifier(RSA publicKey, string description = "")
        {
            // Get RSA parameters
            RSAParameters publicKeyParam = publicKey.ExportParameters(false);

            // Prepare SSH-RSA key components
            byte[] sshrsaBytes = Encoding.Default.GetBytes("ssh-rsa");
            byte[] lengthBytes = BitConverter.GetBytes(sshrsaBytes.Length);
            byte[] exponentBytes = publicKeyParam.Exponent!;
            byte[] exponentBytesLength = BitConverter.GetBytes((UInt32)exponentBytes.Length);
            byte[] keyLength = BitConverter.GetBytes((publicKey.KeySize / 8) + 1);
            byte[] modulusBytes = publicKeyParam.Modulus!;

            // Handle endianness
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
                Array.Reverse(exponentBytesLength);
                Array.Reverse(exponentBytes);
                Array.Reverse(keyLength);
                //Array.Reverse(modulusBytes);
            }

            // Combine the bytes together
            byte[] totalBytes = new byte[lengthBytes.Length + sshrsaBytes.Length + exponentBytes.Length + exponentBytesLength.Length + keyLength.Length + 1 + modulusBytes.Length];
            Buffer.BlockCopy(lengthBytes, 0, totalBytes, 0, lengthBytes.Length);
            Buffer.BlockCopy(sshrsaBytes, 0, totalBytes, lengthBytes.Length, sshrsaBytes.Length);
            Buffer.BlockCopy(exponentBytesLength, 0, totalBytes, lengthBytes.Length + sshrsaBytes.Length, exponentBytesLength.Length);
            Buffer.BlockCopy(exponentBytes, 0, totalBytes, lengthBytes.Length + sshrsaBytes.Length + exponentBytesLength.Length, exponentBytes.Length);
            Buffer.BlockCopy(keyLength, 0, totalBytes, lengthBytes.Length + sshrsaBytes.Length + exponentBytesLength.Length + exponentBytes.Length, keyLength.Length);
            Buffer.BlockCopy(modulusBytes, 0, totalBytes, lengthBytes.Length + sshrsaBytes.Length + exponentBytesLength.Length + exponentBytes.Length + keyLength.Length + 1, modulusBytes.Length);

            // Convert to base64 and format
            string sshKey = Convert.ToBase64String(totalBytes);
            return $"ssh-rsa {sshKey} {description}";
        }

        // Generate SSH identifier from ECDSA public key
        public static string SSHIdentifier(ECDsa publicKey, string description = "")
        {
            // Get ECDSA parameters
            ECParameters publicKeyParam = publicKey.ExportParameters(false);
            string? keyType = null;

            // Determine key type and curve
            switch (publicKeyParam.Curve.Oid.FriendlyName)
            {
                case "nistP256":
                    keyType = "ecdsa-sha2-nistp256";
                    curveBytes = Encoding.Default.GetBytes("nistp256");
                    break;
                case "nistP384":
                    keyType = "ecdsa-sha2-nistp384";
                    curveBytes = Encoding.Default.GetBytes("nistp384");
                    break;
                case "nistP521":
                    keyType = "ecdsa-sha2-nistp521";
                    curveBytes = Encoding.Default.GetBytes("nistp521");
                    break;
                default:
                    throw new Exception("Unknown curve");
            }

            // Prepare ECDSA key components
            byte[] typeBytes = Encoding.Default.GetBytes(keyType);
            byte[] lengthBytes = BitConverter.GetBytes(typeBytes.Length);
            byte keyForm = 0x04;
            byte[] curveByteLength = BitConverter.GetBytes(curveBytes.Length);
            byte[] publicKeyValueQX = publicKeyParam.Q.X!;
            byte[] publicKeyValueQY = publicKeyParam.Q.Y!;
            byte[] publicKeyLength = BitConverter.GetBytes(publicKeyValueQX.Length + publicKeyValueQY.Length);

            // Handle endianness
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
                Array.Reverse(publicKeyLength);
                Array.Reverse(curveByteLength);
                Array.Reverse(curveBytes);
            }

            // Combine key components
            byte[] totalBytes = new byte[lengthBytes.Length + typeBytes.Length + curveByteLength.Length + 
                curveBytes.Length + publicKeyLength.Length + 1 + publicKeyValueQX.Length + publicKeyValueQY.Length];
            
            // Copy components in order
            int offset = 0;
            Buffer.BlockCopy(lengthBytes, 0, totalBytes, offset, lengthBytes.Length);
            offset += lengthBytes.Length;
            Buffer.BlockCopy(typeBytes, 0, totalBytes, offset, typeBytes.Length);
            offset += typeBytes.Length;
            Buffer.BlockCopy(curveByteLength, 0, totalBytes, offset, curveByteLength.Length);
            offset += curveByteLength.Length;
            Buffer.BlockCopy(curveBytes, 0, totalBytes, offset, curveBytes.Length);
            offset += curveBytes.Length;
            Buffer.BlockCopy(publicKeyLength, 0, totalBytes, offset, publicKeyLength.Length);
            offset += publicKeyLength.Length;
            Buffer.SetByte(totalBytes, offset, keyForm);
            offset += 1;
            Buffer.BlockCopy(publicKeyValueQX, 0, totalBytes, offset, publicKeyValueQX.Length);
            offset += publicKeyValueQX.Length;
            Buffer.BlockCopy(publicKeyValueQY, 0, totalBytes, offset, publicKeyValueQY.Length);

            // Convert to base64 and format
            string sshKey = Convert.ToBase64String(totalBytes);
            return $"{keyType} {sshKey} {description}";
        }
    }
}
