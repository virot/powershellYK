using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Export, "YubikeyPIVCertificate")]
    public class ExportYubiKeyPIVCertificateCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to extract", ParameterSetName = "Slot")]
        public byte Slot { get; set; }
        [Alias("AttestationCertificate")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Export Attestation certificate", ParameterSetName = "AttestationCertificate")]
        public SwitchParameter AttestationIntermediateCertificate { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

        protected override void ProcessRecord()
        {
            X509Certificate2 certificate = null;
            if (YubiKeyModule._pivSession is null)
            {
                //throw new Exception("PIV not connected, use Connect-YubikeyPIV first");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyPIV");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            if (AttestationIntermediateCertificate.IsPresent)
            {
                certificate = YubiKeyModule._pivSession.GetAttestationCertificate();
            }
            else
            {
                try
                {
                    certificate = YubiKeyModule._pivSession.GetCertificate(Slot);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get certificate for slot 0x{Slot.ToString("X2")}", e);
                }
            }

            byte[] slotAttestationCertificateBytes = certificate.Export(X509ContentType.Cert);
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