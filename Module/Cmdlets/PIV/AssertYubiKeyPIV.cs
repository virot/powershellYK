using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using powershellYK.PIV;
using powershellYK.support.transform;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Assert, "YubiKeyPIV")]
    public class AssertYubiKeyPIVCommand : PSCmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "ExportToFile")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "DisplayOnScreen")]
        public PIVSlot Slot { get; set; }
        [TransformPath()]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Location of the attestation certificate", ParameterSetName = "ExportToFile")]
        public string? OutFile { get; set; } = null;

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM", ParameterSetName = "DisplayOnScreen")]
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
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                WriteDebug($"Asserting Attestation for slot {Slot}");
                if (pivSession.GetMetadata(Slot).KeyStatus != PivKeyStatus.Generated)
                {
                    throw new NotSupportedException("Can only assert generated keys.");
                }
                X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
                byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);
                if (OutFile is not null)
                {
                    WriteDebug($"Writing Attestation certificate to {OutFile}");
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
