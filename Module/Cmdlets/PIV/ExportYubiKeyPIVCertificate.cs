using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey;
using powershellYK.PIV;
using powershellYK.support.transform;
using powershellYK.support.validators;


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
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

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
                    X509Certificate2? certificate = null;

                    if (AttestationIntermediateCertificate.IsPresent || Slot.ToByte() == PivSlot.Attestation)
                    {
                        certificate = pivSession.GetAttestationCertificate();
                    }
                    else
                    {
                        certificate = pivSession.GetCertificate(Slot);
                    }

                    byte[] slotAttestationCertificateBytes = certificate!.Export(X509ContentType.Cert);
                    string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);
                    if (OutFile is not null)
                    {
                        WriteCommandDetail($"Writing certificate to {OutFile.FullName}");
                        using (FileStream stream = OutFile.OpenWrite())
                        {
                            byte[] pemDataArray = System.Text.Encoding.UTF8.GetBytes(pemData);
                            stream.Write(pemDataArray, 0, pemDataArray.Length);
                        }
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
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "CertificateExportError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
