using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.Security;
using System.Runtime.InteropServices;
using powershellYK.PIV;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Import, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ImportYubiKeyPIVCommand : PSCmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slotnumber")]
        public PIVSlot Slot { get; set; }

        [TransformCertificatePath_Certificate()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate to be stored", ParameterSetName = "CertificateOnly")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate to be stored", ParameterSetName = "CertificateAndKey")]
        public object? Certificate { get; set; } = null;
        [ValidatePath(fileMustExist: true, fileMustNotExist: false, fileExt: ".p12")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "P12 file to be stored", ParameterSetName = "P12")]
        public System.IO.FileInfo? P12Path { get; set; }
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Private key to be stored", ParameterSetName = "Privatekey")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Private key to be stored", ParameterSetName = "CertificateAndKey")]
        public System.IO.FileInfo? PrivateKeyPath { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Private key password", ParameterSetName = "Privatekey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Private key password", ParameterSetName = "CertificateAndKey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Private key password", ParameterSetName = "P12")]
        public SecureString Password { get; set; } = new SecureString();

        [ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy", ParameterSetName = "Privatekey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy", ParameterSetName = "CertificateAndKey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy", ParameterSetName = "P12")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Touch policy", ParameterSetName = "Privatekey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Touch policy", ParameterSetName = "CertificateAndKey")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Touch policy", ParameterSetName = "P12")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;


        private X509Certificate2? _newcertificate = null;
        private PivPrivateKey? _newPrivateKey = null;

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
        protected override void ProcessRecord()
        {
            // Initial section load keys and certificates from files.
            if (ParameterSetName == "CertificateOnly" || ParameterSetName == "CertificateAndKey")
            {
                this._newcertificate = (X509Certificate2)Certificate!;
            }
            if (ParameterSetName == "P12" && P12Path is not null && P12Path.Exists)
            {
                WriteDebug($"Loading P12 from {P12Path}");
                // Make sure to load with the private key exportable
                X509Certificate2 p12Data = new X509Certificate2(P12Path.FullName, Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password!)), X509KeyStorageFlags.Exportable);
                this._newcertificate = p12Data;
                if (p12Data.PublicKey.Oid.FriendlyName == "RSA")
                {
                    WriteDebug("Transforming RSA p12 private key");
                    RSA newRSAPrivateKey;
                    newRSAPrivateKey = p12Data.GetRSAPrivateKey()!;
                    RSAParameters rsaParam = newRSAPrivateKey.ExportParameters(true);
                    PivRsaPrivateKey pivRsaPrivateKey = new PivRsaPrivateKey(
                        rsaParam.P,
                        rsaParam.Q,
                        rsaParam.DP,
                        rsaParam.DQ,
                        rsaParam.InverseQ);
                    this._newPrivateKey = (PivPrivateKey)pivRsaPrivateKey;
                }
                else if (p12Data.PublicKey.Oid.FriendlyName == "ECC")
                {
                    WriteDebug("Transforming ECDsa p12 private key");
                    ECDsa newECDsaPrivateKey;
                    ECParameters eccParam;
                    newECDsaPrivateKey = p12Data.GetECDsaPrivateKey()!;
                    if (newECDsaPrivateKey is ECDsaCng)
                    {
                        throw new NotImplementedException("ECDsaCng not supported");
                        //https://github.com/dotnet/runtime/issues/36899
                    }
                    int keySize = newECDsaPrivateKey.KeySize / 8;
                    eccParam = newECDsaPrivateKey.ExportParameters(true); 
                    byte[] privateKey = new byte[keySize];
                    int offset = keySize - eccParam.D!.Length;
                    Array.Copy(eccParam.D, 0, privateKey, offset, eccParam.D.Length);
                    PivEccPrivateKey pivEccPrivateKey = new PivEccPrivateKey(privateKey);
                    this._newPrivateKey = (PivPrivateKey)pivEccPrivateKey;
                }
            }
            if (ParameterSetName == "Privatekey" || ParameterSetName == "CertificateAndKey")
            {
                string pemContent = "";
                if (PrivateKeyPath!.Exists)
                {
                    using (FileStream fileStream = PrivateKeyPath.OpenRead())
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            pemContent = reader.ReadToEnd();
                        }
                    }
                }
                if (Password.Length >= 1)
                {
                    if (pemContent.Contains("BEGIN ENCRYPTED PRIVATE KEY"))
                    {
                        WriteDebug("Trying to read encrypted RSA key...");
                        RSA newRSAPrivateKey = RSA.Create();
                        newRSAPrivateKey.ImportFromEncryptedPem(pemContent.ToCharArray(), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password!))!));
                        RSAParameters rsaParam = newRSAPrivateKey.ExportParameters(true);
                        PivRsaPrivateKey pivRsaPrivateKey = new PivRsaPrivateKey(
                            rsaParam.P,
                            rsaParam.Q,
                            rsaParam.DP,
                            rsaParam.DQ,
                            rsaParam.InverseQ);
                        this._newPrivateKey = (PivPrivateKey)pivRsaPrivateKey;
                    }
                    else if (pemContent.Contains("BEGIN EC PRIVATE KEY"))
                    {
                        WriteDebug("Trying to read encrypted ECDSA key...");
                        ECDsa newECDsaPrivateKey = ECDsa.Create();
                        newECDsaPrivateKey.ImportFromEncryptedPem(pemContent.ToCharArray(), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Password!))!));
                        int keySize = newECDsaPrivateKey.KeySize / 8;
                        ECParameters eccParam = newECDsaPrivateKey.ExportParameters(true);
                        byte[] privateKey = new byte[keySize];
                        int offset = keySize - eccParam.D!.Length;
                        Array.Copy(eccParam.D, 0, privateKey, offset, eccParam.D.Length);
                        PivEccPrivateKey pivEccPrivateKey = new PivEccPrivateKey(privateKey);
                        this._newPrivateKey = (PivPrivateKey)pivEccPrivateKey;
                    }
                    else
                    {
                        throw new Exception("No private key found in file!");
                    }
                }
                else
                {
                    if (pemContent.Contains("BEGIN PRIVATE KEY"))
                    {
                        try
                        {
                            WriteDebug("Trying to read unencrypted RSA key...");
                            RSA newRSAPrivateKey = RSA.Create();
                            newRSAPrivateKey.ImportFromPem(pemContent.ToCharArray());
                            RSAParameters rsaParam = newRSAPrivateKey.ExportParameters(true);
                            PivRsaPrivateKey pivRsaPrivateKey = new PivRsaPrivateKey(
                                rsaParam.P,
                                rsaParam.Q,
                                rsaParam.DP,
                                rsaParam.DQ,
                                rsaParam.InverseQ);
                            this._newPrivateKey = (PivPrivateKey)pivRsaPrivateKey;
                        }
                        catch { }
                        try
                        {
                            WriteDebug("Trying to read unencrypted ECDSA key...");
                            ECDsa newECDsaPrivateKey = ECDsa.Create();
                            newECDsaPrivateKey.ImportFromPem(pemContent.ToCharArray());
                            int keySize = newECDsaPrivateKey.KeySize / 8;
                            ECParameters eccParam = newECDsaPrivateKey.ExportParameters(true);
                            byte[] privateKey = new byte[keySize];
                            int offset = keySize - eccParam.D!.Length;
                            Array.Copy(eccParam.D, 0, privateKey, offset, eccParam.D.Length);
                            PivEccPrivateKey pivEccPrivateKey = new PivEccPrivateKey(privateKey);
                            this._newPrivateKey = (PivPrivateKey)pivEccPrivateKey;
                        }
                        catch { }
                        if (this._newPrivateKey is null)
                        {
                            throw new Exception("No private key found in file.");
                        }
                    }
                }
            }

            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;


                // If we get a new private key, check and install
                if (_newPrivateKey is not null)
                {
                    PivMetadata? pivMestadata = null;
                    try
                    {
                        pivMestadata = pivSession.GetMetadata(Slot);
                    }
                    catch
                    { }
                    if (pivMestadata is null || pivMestadata.Algorithm == PivAlgorithm.None || ShouldProcess($"Private key into {Slot}", "Import"))
                    {
                        pivSession.ImportPrivateKey(Slot, _newPrivateKey, PinPolicy, TouchPolicy);
                    }
                }

                // If we got a new certificate, check and install
                if (_newcertificate is not null)
                {
                    PivPublicKey? publicKey = null;
                    try
                    {
                        publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    }
                    catch { }
                    X509Certificate2? slotCertificate = null;
                    try
                    {
                        slotCertificate = pivSession.GetCertificate(Slot);
                    }
                    catch { }
                    WriteDebug($"Proceeding with import of thumbprint {_newcertificate.Thumbprint}");

                    // Check that the certificate matches the public key in the slot
                    if (publicKey is null)
                    {
                        throw new Exception("No public key found, not uploading certificate.");
                    }
                    else if ((_newcertificate.PublicKey.Oid.FriendlyName == "RSA" && publicKey is PivEccPublicKey) ||
                        (_newcertificate.PublicKey.Oid.FriendlyName == "ECC" && publicKey is PivRsaPublicKey))
                    {
                        throw new Exception("Private key does match certificate type: RSA / ECDSA.");
                    }
                    else if (publicKey is PivRsaPublicKey)
                    {
                        WriteDebug("Verifying that the RSA key matches the public key...");
                        RSA certificatePublicKey = _newcertificate.GetRSAPublicKey()!;
                        RSA keypublicKey;
                        var rsaParams = new RSAParameters
                        {
                            Modulus = ((PivRsaPublicKey)publicKey).Modulus.ToArray(),
                            Exponent = ((PivRsaPublicKey)publicKey).PublicExponent.ToArray()
                        };
                        keypublicKey = RSA.Create(rsaParams);

                        if (certificatePublicKey.ExportParameters(false).Modulus!.SequenceEqual(keypublicKey.ExportParameters(false).Modulus!) &&
                                               certificatePublicKey.ExportParameters(false).Exponent!.SequenceEqual(keypublicKey.ExportParameters(false).Exponent!))
                        {
                            WriteDebug("Public key matches certificate key.");
                        }
                        else
                        {
                            throw new Exception("Public key DOES NOT match certificate key!");
                        }
                    }
                    else
                    {
                        WriteDebug("Verifying that the ECDSA key matches the public key");
                        using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);
                        ECDsa certificatePublicKey = _newcertificate.GetECDsaPublicKey()!;
                        if (certificatePublicKey.ExportParameters(false).Q.X!.SequenceEqual(((ECDsa)dotNetPublicKey).ExportParameters(false).Q.X!) &&
                                               certificatePublicKey.ExportParameters(false).Q.Y!.SequenceEqual(((ECDsa)dotNetPublicKey).ExportParameters(false).Q.Y!))
                        {
                            WriteDebug("Public key matches certificate key.");
                        }
                        else
                        {
                            throw new Exception("Public key DOES NOT match certificate key!");
                        }
                    }

                    // If we have a certificate in the slot, check if we should overwrite it
                    if (slotCertificate is null || ShouldProcess($"Certificate into {Slot}", "Import"))
                    {
                        pivSession.ImportCertificate(Slot, _newcertificate);
                    }
                }
            }
        }
    }
}
