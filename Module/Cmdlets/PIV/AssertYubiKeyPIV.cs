using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using System.Security.Cryptography;
using powershellYK.support;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.support.transform;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Assert, "YubikeyPIV")]
    public class AssertYubiKeyPIVCommand : PSCmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [TransformPivSlot()]

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "ExportToFile")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "DisplayOnScreen")]
        public byte Slot { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Location of attestation certificate", ParameterSetName = "ExportToFile")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM", ParameterSetName = "DisplayOnScreen")]
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
                X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
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
        }

        protected override void EndProcessing()
        {
        }
    }
}