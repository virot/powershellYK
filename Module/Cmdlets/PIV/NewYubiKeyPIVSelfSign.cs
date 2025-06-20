﻿/// <summary>
/// Creates a self-signed certificate for a key in a specified YubiKey PIV slot.
/// Supports RSA and ECC key types with various hash algorithms.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// New-YubiKeyPIVSelfSign -Slot "PIV Authentication" -Subjectname "CN=Test User"
/// Creates a self-signed certificate for the PIV Authentication slot
/// 
/// .EXAMPLE
/// New-YubiKeyPIVSelfSign -Slot "Digital Signature" -Subjectname "CN=Test User" -HashAlgorithm "SHA384"
/// Creates a self-signed certificate using SHA384 hash algorithm
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.PIV;
using Yubico.YubiKey.Cryptography;
using powershellYK.support;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubiKeyPIVSelfSign", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVSelfSignCommand : Cmdlet
    {
        // Parameter for the PIV slot
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Sign a self-signed certificate for slot")]
        public PIVSlot Slot { get; set; }

        // Parameter for certificate subject name
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]
        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";

        // Parameter for hash algorithm
        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Hash algoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        // Connect to YubiKey when cmdlet starts
        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected.");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                CertificateRequest request;
                X509SignatureGenerator signer;
                X500DistinguishedName dn;
                X509SubjectKeyIdentifierExtension certificateSKI;

                IPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKeyParameters;
                    if (publicKey is null)
                    {
                        throw new Exception("Public key is null!");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does the key exist?", e);
                }

                // Create certificate request based on key type
                WriteDebug($"Starting to generate CertificateRequest for KeyType {publicKey.KeyType.ToString()}");
                if (publicKey is RSAPublicKey rsaPublicKey)
                {
                    request = new CertificateRequest(Subjectname, (RSA)Converter.YubiKeyPublicKeyToDotNet(publicKey), HashAlgorithm, RSASignaturePadding.Pkcs1);
                }
                else if (publicKey is ECPublicKey ecPublicKey)
                {
                    // Set hash algorithm based on ECC key size
                    HashAlgorithm = ecPublicKey.KeyType switch
                    {
                        KeyType.ECP256 => HashAlgorithmName.SHA256,
                        KeyType.ECP384 => HashAlgorithmName.SHA384,
                        KeyType.ECP521 => HashAlgorithmName.SHA512,
                        _ => throw new Exception("Unknown public Key algorithm")
                    };
                    WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");

                    request = new CertificateRequest(Subjectname, ((ECDsa)Converter.YubiKeyPublicKeyToDotNet(publicKey)), HashAlgorithm);
                }
                else if (publicKey is Curve25519PublicKey)
                {
                    throw new Exception("Unimplemented public Key algorithm, Curve25519PublicKey");
                }
                else
                {
                    throw new Exception("Unknown public Key algorithm");
                }

                // Add certificate extensions
                X509BasicConstraintsExtension x509BasicConstraintsExtension = new X509BasicConstraintsExtension(true, true, 2, true);
                X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, false);
                request.CertificateExtensions.Add(x509BasicConstraintsExtension);
                request.CertificateExtensions.Add(x509KeyUsageExtension);

                // Add Subject Key Identifier (SKI)
                certificateSKI = new X509SubjectKeyIdentifierExtension(new System.Security.Cryptography.X509Certificates.PublicKey(Converter.YubiKeyPublicKeyToDotNet(publicKey)), false);
                request.CertificateExtensions.Add(certificateSKI);

                // Set certificate validity period
                DateTimeOffset notBefore = DateTimeOffset.Now;
                DateTimeOffset notAfter = notBefore.AddYears(10);
                byte[] serialNumber = new byte[] { 0x01 };

                // Create signature generator based on key type
                if (publicKey is RSAPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else if (publicKey is ECPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }
                else if (publicKey is Curve25519PublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }
                else
                {
                    throw new Exception("Unknown public Key algorithm");
                }

                // Create distinguished name
                try
                {
                    dn = new X500DistinguishedName(Subjectname);
                }
                catch (Exception e) { throw new Exception("Failed to create X500DistinguishedName", e); }

                // Create and import certificate
                X509Certificate2 selfCert = request.Create(dn, signer, notBefore, notAfter, serialNumber);

                // Check if certificate already exists
                bool certExists = false;
                try
                {
                    X509Certificate2 certtest = pivSession.GetCertificate(Slot);
                    certExists = true;
                }
                catch { }

                // Import certificate if slot is empty or user confirms
                if (!certExists || ShouldProcess($"Certificate in slot {Slot}", "New"))
                {
                    WriteDebug($"Importing created certificate into YubiKey slot {Slot}");
                    pivSession.ImportCertificate(Slot, selfCert);
                }
                WriteDebug("ProcessRecord in New-YubikeyPIVSelfSign");
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
