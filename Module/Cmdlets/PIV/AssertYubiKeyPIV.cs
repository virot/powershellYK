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
    public class AssertYubiKeyPIVCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot")]

        public byte Slot { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Location of attestation certificate")]
        public string? OutFile { get; set; } = null;

        protected override void ProcessRecord()
        {
            X509Certificate2 slotAttestationCertificate = YubiKeyModule._pivSession.CreateAttestationStatement(Slot);
            byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
            string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);
            if (OutFile is not null)
            {
                File.WriteAllText(OutFile, pemData);
            }
            else
            {
                WriteObject(pemData);
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}