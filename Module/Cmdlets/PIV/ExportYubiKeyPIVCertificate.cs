using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey;
using powershellYK.support.transform;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Export, "YubikeyPIVCertificate")]
    public class ExportYubiKeyPIVCertificateCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to extract", ParameterSetName = "Slot")]
        public byte Slot { get; set; }
        [Alias("AttestationCertificate")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Export Attestation certificate", ParameterSetName = "AttestationCertificate")]
        public SwitchParameter AttestationIntermediateCertificate { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

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
                X509Certificate2? certificate = null;

                if (AttestationIntermediateCertificate.IsPresent)
                {
                    certificate = pivSession.GetAttestationCertificate();
                }
                else
                {
                    try
                    {
                        certificate = pivSession.GetCertificate(Slot);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to get certificate for slot 0x{Slot.ToString("X2")}", e);
                    }
                }

                byte[] slotAttestationCertificateBytes = certificate!.Export(X509ContentType.Cert);
                string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);

                if (OutFile is not null)
                {
                    File.WriteAllText(OutFile, pemData);
                }
                else
                {
                    if (PEMEncoded.IsPresent)
                    {
                        WriteObject(pemData);
                    }
                    else
                    {
                        WriteObject(certificate);
                    }
                }
            }
        }
    }
}