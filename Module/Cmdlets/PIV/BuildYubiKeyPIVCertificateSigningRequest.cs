using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using System.Security.Cryptography;
using Yubico.YubiKey.Sample.PivSampleCode;
using powershellYK.PIV;
using powershellYK.support.transform;
using powershellYK.support.validators;


namespace powershellYK.Cmdlets.PIV
{
    [Alias("New-YubikeyPIVCSR")]
    [Cmdlet(VerbsLifecycle.Build, "YubiKeyPIVCertificateSigningRequest")]
    public class BuildYubiKeyPIVCertificateSigningRequestCmdlet : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot", ParameterSetName = "With Attestation")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Create a CSR for slot", ParameterSetName = "Without Attestation")]
        public PIVSlot Slot { get; set; }
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Include attestation certificate in CSR", ParameterSetName = "With Attestation")]
        public SwitchParameter Attestation { get; set; }
        [ValidateSet("Both", "Legacy", "Standard", ErrorMessage = null)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "OID to store attestation in CSR.", ParameterSetName = "With Attestation")]
        public string AttestationLocation { get; set; } = "Both";
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Subject name of certificate")]

        public string Subjectname { get; set; } = "CN=SubjectName to be supplied by Server,O=Fake";
        [TransformPath]
        [ValidatePath(fileMustExist: false, fileMustNotExist: true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Save CSR as file")]
        public System.IO.FileInfo? OutFile { get; set; } = null;

        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "HashAlgoritm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

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
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                CertificateRequest request;
                X509SignatureGenerator signer;

                // This is only supported on firmware 5.3.0 and newer.
                if (((YubiKeyDevice)YubiKeyModule._yubikey!).FirmwareVersion < new FirmwareVersion(5, 3, 0))
                {
                    throw new NotSupportedException("This feature requires firmware version 5.3.0 or newer.");
                }
                
                // get the metadata catch if fails
                PivMetadata? metadata = null;
                PivPublicKey? publicKey = null;
                try
                {
                    metadata = pivSession.GetMetadata(Slot);
                    publicKey = metadata.PublicKey;
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get metadata for slot {Slot}.", e);
                }

                if (publicKey is null)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does there exist a key?");
                }
                if (Attestation.IsPresent)
                {
                    if (metadata.KeyStatus != PivKeyStatus.Generated)
                    {
                        throw new InvalidOperationException($"Private key must be generated on YubiKey for attested certificate requests. {Slot} is {metadata.KeyStatus}.");
                    }
                }


                using AsymmetricAlgorithm dotNetPublicKey = KeyConverter.GetDotNetFromPivPublicKey(publicKey);

                if (publicKey is PivRsaPublicKey)
                {
                    request = new CertificateRequest(Subjectname, (RSA)dotNetPublicKey, HashAlgorithm, RSASignaturePadding.Pkcs1);
                }
                else
                {
                    HashAlgorithm = publicKey.Algorithm switch
                    {
                        PivAlgorithm.EccP256 => HashAlgorithmName.SHA256,
                        PivAlgorithm.EccP384 => HashAlgorithmName.SHA384,
                        _ => throw new Exception("Unknown Public key algorithm")
                    };
                    WriteDebug($"Using Hash based on ECC size: {HashAlgorithm.ToString()}");
                    request = new CertificateRequest(Subjectname, (ECDsa)dotNetPublicKey, HashAlgorithm);
                }

                if (Attestation.IsPresent)
                {
                    X509Certificate2 slotAttestationCertificate = pivSession.CreateAttestationStatement(Slot);
                    byte[] slotAttestationCertificateBytes = slotAttestationCertificate.Export(X509ContentType.Cert);
                    X509Certificate2 yubikeyIntermediateAttestationCertificate = pivSession.GetAttestationCertificate();
                    byte[] yubikeyIntermediateAttestationCertificateBytes = yubikeyIntermediateAttestationCertificate.Export(X509ContentType.Cert);
                    Oid oidIntermediate = new Oid("1.3.6.1.4.1.41482.3.2");
                    Oid oidSlotAttestationStandard = new Oid("1.3.6.1.4.1.41482.3.1");
                    Oid oidSlotAttestationLegacy = new Oid("1.3.6.1.4.1.41482.3.11");
                    switch (AttestationLocation)
                    {
                        case "Both":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationStandard, slotAttestationCertificateBytes, false));
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationLegacy, slotAttestationCertificateBytes, false));
                            break;
                        case "Standard":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationStandard, slotAttestationCertificateBytes, false));
                            break;
                        case "Legacy":
                            request.CertificateExtensions.Add(new X509Extension(oidSlotAttestationLegacy, slotAttestationCertificateBytes, false));
                            break;
                    }
                    request.CertificateExtensions.Add(new X509Extension(oidIntermediate, yubikeyIntermediateAttestationCertificateBytes, false));
                }

                if (publicKey is PivRsaPublicKey)
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                }
                else
                {
                    signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                }

                byte[] requestSigned = request.CreateSigningRequest(signer);
                string pemData = PemEncoding.WriteString("CERTIFICATE REQUEST", requestSigned);
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
                        WriteObject(requestSigned);
                    }
                }
                WriteDebug("ProcessRecord in New-YubikeyPIVCSR");
            }
        }


        protected override void EndProcessing()
        {
        }
    }
}
