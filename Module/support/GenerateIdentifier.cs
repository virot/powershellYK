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

namespace VirotYubikey.support
{
    internal class GenerateIdentifier
    {
        private static byte[] curveBytes;

        public static string SSHIdentifier(RSA publicKey, string description = "")
        {
            RSAParameters publicKeyParam = publicKey.ExportParameters(false);

            // SSH-RSA keys are encoded in a specific format
            byte[] sshrsaBytes = Encoding.Default.GetBytes("ssh-rsa");
            byte[] lengthBytes = BitConverter.GetBytes(sshrsaBytes.Length);
            byte[] exponentBytes = publicKeyParam.Exponent;
            byte[] exponentBytesLength = BitConverter.GetBytes((UInt32)exponentBytes.Length);
            byte[] keyLength = BitConverter.GetBytes((publicKey.KeySize / 8) + 1);
            byte[] modulusBytes = publicKeyParam.Modulus;
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

            // Convert to base64
            string sshKey = Convert.ToBase64String(totalBytes);

            // Format as an SSH-RSA key
            return $"ssh-rsa {sshKey} {description}";
        }
        public static string SSHIdentifier(ECDsa publicKey, string description = "")
        {
    
            ECParameters publicKeyParam = publicKey.ExportParameters(false);
            byte[]? curvebytes = null;
            string? keyType = null;
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
            byte[] typeBytes = Encoding.Default.GetBytes(keyType);
            byte[] lengthBytes = BitConverter.GetBytes(typeBytes.Length);
            byte keyForm = 0x04;
            byte[] curveByteLength = BitConverter.GetBytes(curveBytes.Length);
            byte[] publicKeyValueQX = publicKeyParam.Q.X;
            byte[] publicKeyValueQY = publicKeyParam.Q.Y;
            byte[] publicKeyLength = BitConverter.GetBytes(publicKeyValueQX.Length+ publicKeyValueQY.Length);
            byte[] cofactor = publicKeyParam.Curve.Cofactor;
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
                Array.Reverse(publicKeyLength);
                Array.Reverse(curveByteLength);
                Array.Reverse(curveBytes);
            }

            //return $"QX:{(publicKeyParam.Q.X)[0]}, QY:{(publicKeyParam.Q.Y)[0]} ";

            // Combine the bytes together
            byte[] totalBytes = new byte[lengthBytes.Length + typeBytes.Length + curveByteLength.Length + 8 + publicKeyLength.Length + 1 + publicKeyValueQX.Length + publicKeyValueQY.Length];
            Buffer.BlockCopy(lengthBytes, 0, totalBytes, 0, lengthBytes.Length);
            Buffer.BlockCopy(typeBytes, 0, totalBytes, lengthBytes.Length, typeBytes.Length);
            Buffer.BlockCopy(curveByteLength, 0, totalBytes, lengthBytes.Length + typeBytes.Length, curveByteLength.Length);
            Buffer.BlockCopy(curveBytes, 0, totalBytes, lengthBytes.Length + typeBytes.Length + curveByteLength.Length, curveBytes.Length);
            Buffer.BlockCopy(publicKeyLength, 0, totalBytes, lengthBytes.Length + typeBytes.Length + curveByteLength.Length + curveBytes.Length, publicKeyLength.Length);
            Buffer.SetByte(totalBytes, lengthBytes.Length + typeBytes.Length + curveByteLength.Length + curveBytes.Length + publicKeyLength.Length, keyForm);
            Buffer.BlockCopy(publicKeyValueQX, 0, totalBytes, lengthBytes.Length + typeBytes.Length + curveByteLength.Length + curveBytes.Length + publicKeyLength.Length + 1, publicKeyValueQX.Length);
            Buffer.BlockCopy(publicKeyValueQY, 0, totalBytes, lengthBytes.Length + typeBytes.Length + curveByteLength.Length + curveBytes.Length + publicKeyLength.Length + 1 + publicKeyValueQX.Length, publicKeyValueQY.Length);
            //Buffer.BlockCopy(Encoding.Default.GetBytes(curveName), 0, totalBytes, keyType.Length, curveName.Length);
            //Buffer.BlockCopy(publicKeyValue, 0, totalBytes, keyType.Length + curveName.Length, publicKeyValue.Length);

            // Convert to base64
            string sshKey = Convert.ToBase64String(totalBytes);

            // Format as an SSH-RSA key
            return $"{keyType} {sshKey} {description}";
        }
    }
}
