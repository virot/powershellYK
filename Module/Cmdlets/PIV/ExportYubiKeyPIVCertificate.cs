/// <summary>
/// Exports a certificate or attestation certificate from a specified YubiKey PIV slot.
/// Supports exporting as PEM or binary, and saving to a file.
/// Requires a YubiKey with PIV support and a valid certificate in the slot.
/// 
/// .EXAMPLE
/// Export-YubiKeyPIVCertificate -Slot "PIV Authentication" -OutFile "cert.cer"
/// Exports the certificate from the PIV Authentication slot to a file
/// 
/// .EXAMPLE
/// Export-YubiKeyPIVCertificate -AttestationIntermediateCertificate -PEMEncoded
/// Exports the attestation certificate as PEM
/// </summary>

// Imports
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
        // Parameter for the PIV slot to export certificate from
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Slot to extract", ParameterSetName = "Slot")]
        public PIVSlot Slot { get; set; }

        // Parameter to export the attestation certificate instead of slot certificate
        [Alias("AttestationCertificate")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Export Attestation certificate", ParameterSetName = "AttestationCertificate")]
        public SwitchParameter AttestationIntermediateCertificate { get; set; }

        // Parameter for the output file path
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Output file")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

        // Parameter to output certificate in PEM format
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM")]
        public SwitchParameter PEMEncoded { get; set; }

        // Called when the cmdlet begins processing
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
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

        // Main logic for exporting the certificate
        protected override void ProcessRecord()
        {
            try
            {
                // Open a session with the YubiKey PIV application
                using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
                {
                    // Set up key collector for PIN entry
                    pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                    X509Certificate2? certificate = null;

                    // Determine which certificate to export
                    if (AttestationIntermediateCertificate.IsPresent || Slot.ToByte() == PivSlot.Attestation)
                    {
                        // Export the attestation certificate
                        certificate = pivSession.GetAttestationCertificate();
                    }
                    else
                    {
                        // Export the certificate from the specified slot
                        certificate = pivSession.GetCertificate(Slot);
                    }

                    // Export the certificate in DER format
                    byte[] slotAttestationCertificateBytes = certificate!.Export(X509ContentType.Cert);
                    
                    // Convert to PEM format if needed
                    string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);

                    // Handle output based on parameters
                    if (OutFile is not null)
                    {
                        // Write certificate to file
                        WriteCommandDetail($"Writing certificate to {OutFile.FullName}");
                        using (FileStream stream = OutFile.OpenWrite())
                        {
                            byte[] pemDataArray = System.Text.Encoding.UTF8.GetBytes(pemData);
                            stream.Write(pemDataArray, 0, pemDataArray.Length);
                        }
                    }
                    else
                    {
                        // Output to pipeline
                        if (PEMEncoded.IsPresent)
                        {
                            // Output as PEM string
                            WriteObject(pemData);
                        }
                        else
                        {
                            // Output as certificate object
                            WriteObject(certificate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors during export
                WriteError(new ErrorRecord(ex, "CertificateExportError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
