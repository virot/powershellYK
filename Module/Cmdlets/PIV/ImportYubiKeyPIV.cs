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
using powershellYK.support.transform;
using powershellYK.support.validators;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Import, "YubikeyPIV")]
    public class ImportYubiKeyPIVCommand : PSCmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [ValidateX509Certificate2_string()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slotnumber")]
        public byte Slot { get; set; }

        [TransformCertificatePath_Certificate()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Certificate to be stored", ParameterSetName = "CertificateOnly")]
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

                switch (ParameterSetName)
                {
                    case "CertificateOnly":
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
                        if (Certificate is null)
                        {
                            throw new ArgumentException("No certificate found");
                        }
                        _certificate = (X509Certificate2)Certificate!;
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
                        break;
                    case "KeyOnly":
                        break;

                    case "P12":
                        break;
                }
            }
        }
    }
}