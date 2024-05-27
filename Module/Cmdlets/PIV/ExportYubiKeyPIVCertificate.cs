using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsData.Export, "YubikeyPIVCertificate")]
    public class ExportYubiKeyPIVCertificateCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to extract", ParameterSetName = "Slot")]
        public byte Slot { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Export Attestation certificate", ParameterSetName = "AttestationCertificate")]
        public SwitchParameter AttestationCertificate { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public string? OutFile { get; set; } = null;

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

            if (AttestationCertificate.IsPresent)
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

            if (OutFile is not null)
            {
                string base64content = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));
                File.WriteAllText(OutFile, base64content);
            }
            else
            {
                WriteObject(certificate);
            }
        }
    }
}