using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey;
using powershellYK.PIV;
using powershellYK.support.transform;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Export, "YubiKeyPIVCertificate")]
    public class ExportYubiKeyPIVCertificateCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to extract", ParameterSetName = "Slot")]
        public PIVSlot Slot { get; set; }
        [Alias("AttestationCertificate")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Export Attestation certificate", ParameterSetName = "AttestationCertificate")]
        public SwitchParameter AttestationIntermediateCertificate { get; set; }
        [TransformPath()]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

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
            try
            {
                using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    X509Certificate2 cert;

                    // Special handling for attestation slot F9
                    if (Slot == 0xF9)
                    {
                        cert = pivSession.GetAttestationCertificate();
                    }
                    else
                    {
                        cert = pivSession.GetCertificate(Slot);
                    }

                    // Export the certificate
                    if (OutFile != null)
                    {
                        File.WriteAllText(OutFile, ExportCertificate(cert));
                    }
                    else
                    {
                        WriteObject(ExportCertificate(cert));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "CertificateExportError", ErrorCategory.OperationStopped, null));
            }
        }

        private string ExportCertificate(X509Certificate2 cert)
        {
            byte[] slotAttestationCertificateBytes = cert.Export(X509ContentType.Cert);
            string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);
            return pemData;
        }
    }
}