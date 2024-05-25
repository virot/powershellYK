using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yubico.YubiKey.Piv;
using VirotYubikey;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Yubico.YubiKey.Cryptography;

namespace VirotYubikey.support
{
    public sealed class YubiKeySignatureGenerator : X509SignatureGenerator
    {
        private readonly PivSession _pivSession;
        private readonly byte _slotNumber;
        private readonly int _keySizeBits;

        private readonly X509SignatureGenerator _defaultGenerator;
        private readonly RSASignaturePaddingMode _paddingMode;

        public YubiKeySignatureGenerator(
            PivSession pivSession,
            byte slotNumber,
            RSA rsaPublicKeyObject,
            RSASignaturePadding paddingScheme)
        {
            _pivSession = pivSession;
            _slotNumber = slotNumber;
            _keySizeBits = rsaPublicKeyObject.KeySize;
            _defaultGenerator = X509SignatureGenerator.CreateForRSA(rsaPublicKeyObject, paddingScheme);
            _paddingMode = paddingScheme.Mode;
        }

        protected override PublicKey BuildPublicKey()
        {
            return _defaultGenerator.PublicKey;
        }

        public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
        {
            return _defaultGenerator.GetSignatureAlgorithmIdentifier(hashAlgorithm);
        }

        public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
        {
            byte[] dataToSign = DigestData(data, hashAlgorithm);
            dataToSign = PadRsa(dataToSign, hashAlgorithm);

            return _pivSession.Sign(_slotNumber, dataToSign);
        }

        private byte[] DigestData(byte[] dataToDigest, HashAlgorithmName hashAlgorithm)
        {
            using HashAlgorithm digester = hashAlgorithm.Name switch
            {
                "SHA1" => CryptographyProviders.Sha1Creator(),
                "SHA256" => CryptographyProviders.Sha256Creator(),
                "SHA384" => CryptographyProviders.Sha384Creator(),
                "SHA512" => CryptographyProviders.Sha512Creator(),
                _ => throw new ArgumentException(),
            };
            byte[] digest = new byte[digester.HashSize / 8];

            _ = digester.TransformFinalBlock(dataToDigest, 0, dataToDigest.Length);
            Array.Copy(digester.Hash, 0, digest, 0, digest.Length);

            return digest;
        }

        private byte[] PadRsa(byte[] digest, HashAlgorithmName hashAlgorithm)
        {
            int digestAlgorithm = hashAlgorithm.Name switch
            {
                "SHA1" => RsaFormat.Sha1,
                "SHA256" => RsaFormat.Sha256,
                "SHA384" => RsaFormat.Sha384,
                "SHA512" => RsaFormat.Sha512,
                _ => 0,
            };

            if (_paddingMode == RSASignaturePaddingMode.Pkcs1)
            {
                return RsaFormat.FormatPkcs1Sign(digest, digestAlgorithm, _keySizeBits);
            }

            return RsaFormat.FormatPkcs1Pss(digest, digestAlgorithm, _keySizeBits);
        }
    }
}
