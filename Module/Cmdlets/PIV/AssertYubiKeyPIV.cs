/// <summary>
/// Creates an attestation statement for a key in a specified YubiKey PIV slot.
/// Can output the attestation certificate to a file or display it on screen.
/// Requires a YubiKey with PIV support and a generated key in the slot.
/// 
/// .EXAMPLE
/// Assert-YubiKeyPIV -Slot "PIV Authentication" -OutFile "attestation.cer"
/// Creates an attestation certificate for the PIV Authentication slot and saves it to a file
/// 
/// .EXAMPLE
/// Assert-YubiKeyPIV -Slot "Digital Signature" -PEMEncoded
/// Creates an attestation certificate and displays it in PEM format
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using powershellYK.PIV;
using powershellYK.support.transform;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsLifecycle.Assert, "YubiKeyPIV")]
    public class AssertYubiKeyPIVCommand : PSCmdlet
    {
        // Parameter for the PIV slot
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "ExportToFile")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Yubikey PIV Slot", ParameterSetName = "DisplayOnScreen")]
        public PIVSlot Slot { get; set; }

        // Parameter for output file
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Location of the attestation certificate", ParameterSetName = "ExportToFile")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

        // Parameter for PEM encoding
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Encode output as PEM", ParameterSetName = "DisplayOnScreen")]
        public SwitchParameter PEMEncoded { get; set; }

        // Connect to YubiKey when cmdlet starts
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

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for authentication
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Verify key status and create attestation
                WriteDebug($"Asserting Attestation for slot {Slot}");
                if (pivSession.GetMetadata(Slot).KeyStatus != PivKeyStatus.Generated)
                {
                    throw new NotSupportedException("Can only assert generated keys.");
                }

                // Create and format attestation certificate
                X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
                byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                string pemData = PemEncoding.WriteString("CERTIFICATE", slotAttestationCertificateBytes);

                // Output attestation certificate
                if (OutFile is not null)
                {
                    WriteDebug($"Writing Attestation certificate to {OutFile.FullName}");
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
                        WriteObject(slotAttestationCertificate);
                    }
                }
            }
        }

        // Clean up resources when cmdlet ends
        protected override void EndProcessing()
        {
        }
    }
}
