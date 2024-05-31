using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using VirotYubikey.support;
using Yubico.YubiKey.Sample.PivSampleCode;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Assert, "YubikeyPIV")]
    public class AssertYubiKeyPIVCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "ExportToFile")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "DisplayOnScreen")]
        public byte Slot { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Location of attestation certificate", ParameterSetName = "ExportToFile")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM", ParameterSetName = "DisplayOnScreen")]
        public SwitchParameter PEMEncoded { get; set; }


        protected override void ProcessRecord()
        {
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

            X509Certificate2 slotAttestationCertificate = YubiKeyModule._pivSession.CreateAttestationStatement(Slot);
            byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
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
                    WriteObject(slotAttestationCertificate);
                }
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}