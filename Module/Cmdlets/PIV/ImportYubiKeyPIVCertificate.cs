using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using powershellYK.support;
using System.Linq.Expressions;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Import, "YubikeyPIVCertificate")]
    public class ImportYubiKeyPIVCertificateCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slotnumber")]
        public byte Slot { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "File")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Path to certificate")]
        public string? Path { get; set; } = null;

        [Parameter(Mandatory = true, ParameterSetName = "Value")]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Certificate to be stored")]
        public object? Certificate { get; set; } = null;

        private X509Certificate2? _certificate = null;

        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
        }
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                PivPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot 0x{Slot.ToString("X2")}", e);
                }

                WriteDebug("Entering certificate load section");
                if (Path is not null)
                {
                    try
                    {
                        WriteDebug($"Reading PEM certificate from '{Path}'");
                        _certificate = new X509Certificate2(Path);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to load certificate", e);
                    }
                }
                else if (Certificate is not null)
                {
                    if (Certificate.GetType() == typeof(X509Certificate2))
                    {
                        WriteDebug("Just taking the object passed");
                        _certificate = (X509Certificate2)Certificate;
                    }
                    else if (Certificate.GetType() == typeof(string))
                    {
                        try
                        {
                            WriteDebug("Put bytes of certificate into X509Certificate2");
                            _certificate = new X509Certificate2(Encoding.UTF8.GetBytes((string)Certificate));
                        }
                        catch
                        {
                            throw new Exception("Failed to load certificate");
                        }
                    }
                    else
                    {
                        throw new Exception("Certificate is not a valid type");
                    }
                }
                else
                {
                    WriteDebug("Unable to guess type of certificate");
                    throw new Exception("Certificate is not a valid type");
                }
                WriteDebug($"Proceeding with import of thumbprint {_certificate.Thumbprint}");
                if (publicKey is null)
                {
                    throw new Exception("No public key found, not uploading certificate");
                }
                else if (publicKey is PivRsaPublicKey)
                {
                    WriteDebug("Verifying that the key matches the public key");
                    RSA certificatePublicKey = _certificate.GetRSAPublicKey()!;
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
                        WriteDebug("Public key matches certificate key");
                    }
                    else
                    {
                        throw new Exception("Public key does not match certificate key");
                    }
                }
                else
                {
                    using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);
                    ECDsa certificatePublicKey = _certificate.GetECDsaPublicKey()!;
                    if (certificatePublicKey.ExportParameters(false).Q.X!.SequenceEqual(((ECDsa)dotNetPublicKey).ExportParameters(false).Q.X!) &&
                                           certificatePublicKey.ExportParameters(false).Q.Y!.SequenceEqual(((ECDsa)dotNetPublicKey).ExportParameters(false).Q.Y!))
                    {
                        WriteDebug("Public key matches certificate key");
                    }
                    else
                    {
                        throw new Exception("Public key does not match certificate key");
                    }
                }

                try
                {
                    pivSession.ImportCertificate(Slot, _certificate);
                    WriteDebug("Upload complete");
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to import certificate", e);
                }
            }
        }
    }
}