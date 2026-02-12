/// <summary>
/// Verifies the attestation of a YubiKey FIDO2 credential.
/// Validates that the FIDO2 passkey provided was generated on the YubiKey.
/// Reads attestation files in OpenSSH ssh-sk-attest-v01 format (e.g. from ssh-keygen -O write-attestation=attestation.bin).
/// Checks that the attestation certificate is signed by a Yubico FIDO root CA and that the CA is valid.
/// 
/// .EXAMPLE
/// Confirm-YubiKeyFIDO2Attestation
/// Verifies attestation using the default file name "attestation.bin" in the current directory.
/// 
/// .EXAMPLE
/// Confirm-YubiKeyFIDO2Attestation -AttestationObject "attestation.bin"
/// Verifies attestation of a FIDO2 credential using an attestation object file.
/// </summary>

// Imports
using powershellYK.FIDO2;
using powershellYK.support.transform;
using powershellYK.support.validators;
using System.Management.Automation;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace powershellYK.Cmdlets.Fido
{
    [Cmdlet(VerbsLifecycle.Confirm, "YubiKeyFIDO2Attestation")]
    public class ConfirmYubikeyFIDO2AttestationCmdlet : PSCmdlet
    {
        // Parameter for the attestation file (OpenSSH ssh-sk-attest-v01 format)
        [TransformPath]
        [ValidatePath(fileMustExist: true, fileMustNotExist: false)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Path to attestation file (e.g. attestation.bin from ssh-keygen -O write-attestation)")]
        public System.IO.FileInfo? AttestationObject { get; set; }

        // Called when the cmdlet begins processing
        protected override void BeginProcessing()
        {
        }

        // Main logic for verifying FIDO2 attestation
        protected override void ProcessRecord()
        {
            // Default to attestation.bin in current directory when not specified
            System.IO.FileInfo attestationFile = AttestationObject ?? new System.IO.FileInfo(
                System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, "attestation.bin"));

            if (!attestationFile.Exists)
            {
                throw new ArgumentException($"Attestation file not found: {attestationFile.FullName}. Specify -AttestationObject or ensure attestation.bin exists.");
            }

            WriteDebug($"Reading attestation from {attestationFile.FullName}");

            byte[] attestationBytes;
            using (var fs = attestationFile.OpenRead())
            {
                attestationBytes = new byte[fs.Length];
                _ = fs.Read(attestationBytes, 0, attestationBytes.Length);
            }

            X509Certificate2 attestationCertificate = ParseSshSkAttestation(attestationBytes);

            WriteDebug($"Attestation certificate: Subject=\"{attestationCertificate.Subject}\", Issuer=\"{attestationCertificate.Issuer}\", NotBefore={attestationCertificate.NotBefore.ToString("O")}, NotAfter={attestationCertificate.NotAfter.ToString("O")}");

            // Build and verify the certificate chain using Yubico FIDO roots and intermediates
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            // Load Yubico FIDO roots and intermediates from embedded resources
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            const string fidoRootsResource = "powershellYK.support.AttestationCerts.CA.yubico-fido-roots.pem";
            // This PEM file contains both Yubico PIV and FIDO intermediate certificates(!)
            const string fidoIntermediateResource = "powershellYK.support.AttestationCerts.Intermediate.yubico-intermediate.pem";

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.Equals(fidoRootsResource, StringComparison.Ordinal))
                {
                    using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream is not null)
                        {
                            byte[] resourceBytes = new byte[resourceStream.Length];
                            _ = resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                            chain.ChainPolicy.CustomTrustStore.ImportFromPem(Encoding.UTF8.GetString(resourceBytes).AsSpan());
                            WriteDebug($"Loaded {chain.ChainPolicy.CustomTrustStore.Count} root CA(s) from {fidoRootsResource}");
                        }
                    }
                }
                else if (resourceName.Equals(fidoIntermediateResource, StringComparison.Ordinal))
                {
                    using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream is not null)
                        {
                            byte[] resourceBytes = new byte[resourceStream.Length];
                            _ = resourceStream.Read(resourceBytes, 0, resourceBytes.Length);
                            chain.ChainPolicy.ExtraStore.ImportFromPem(Encoding.UTF8.GetString(resourceBytes).AsSpan());
                            WriteDebug($"Loaded {chain.ChainPolicy.ExtraStore.Count} intermediate(s) from {fidoIntermediateResource}");
                        }
                    }
                }
            }

            foreach (X509Certificate2 root in chain.ChainPolicy.CustomTrustStore)
            {
                WriteDebug($"  Root CA: Subject=\"{root.Subject}\", NotBefore={root.NotBefore.ToString("O")}, NotAfter={root.NotAfter.ToString("O")}, Valid={DateTime.UtcNow >= root.NotBefore.ToUniversalTime() && DateTime.UtcNow <= root.NotAfter.ToUniversalTime()}");
            }

            if (chain.ChainPolicy.CustomTrustStore.Count == 0)
            {
                throw new InvalidOperationException("Yubico FIDO roots could not be loaded from embedded resources.");
            }

            // Verify the attestation certificate chain (validates signature by CA and CA validity)
            bool chainValid = chain.Build(attestationCertificate);
            WriteDebug($"Chain build result: {(chainValid ? "Valid" : "Invalid")}");

            if (!chainValid)
            {
                for (int i = 0; i < chain.ChainElements.Count; i++)
                {
                    var elem = chain.ChainElements[i];
                    string statuses = string.Join(", ", elem.ChainElementStatus.Select(s => s.StatusInformation ?? ""));
                    string statusInfo = string.IsNullOrEmpty(statuses) ? "None" : statuses;
                    WriteDebug($"  ChainElement[{i}]: Subject=\"{elem.Certificate.Subject}\" StatusFlags={statusInfo}");
                }
                FIDO2AttestationResult returnObject = new FIDO2AttestationResult(false, attestationPath: null);
                WriteObject(returnObject);
                return;
            }

            // Extract the attestation path (replicated logic as PIV cmdlet)
            List<string> attestationPath = chain.ChainElements.Cast<X509ChainElement>().Skip(2).Select(e => e.Certificate.Subject.ToString()).Where(subj => subj != null).Select(item => item!).ToList();
            attestationPath.Reverse();

            X509Certificate2? matchedRoot = chain.ChainElements.Count > 0 ? chain.ChainElements[^1].Certificate : null;
            WriteDebug($"Matched root certificate: Subject=\"{matchedRoot?.Subject}\", NotBefore={matchedRoot?.NotBefore.ToString("O")}, NotAfter={matchedRoot?.NotAfter.ToString("O")}");
            for (int i = 0; i < chain.ChainElements.Count; i++)
            {
                var elem = chain.ChainElements[i];
                bool valid = DateTime.UtcNow >= elem.Certificate.NotBefore.ToUniversalTime() && DateTime.UtcNow <= elem.Certificate.NotAfter.ToUniversalTime();
                WriteDebug($"  Chain[{i}]: Subject=\"{elem.Certificate.Subject}\" Issuer=\"{elem.Certificate.Issuer}\" Valid={valid} (NotBefore={elem.Certificate.NotBefore.ToString("O")}, NotAfter={elem.Certificate.NotAfter.ToString("O")})");
            }

            FIDO2AttestationResult result = new FIDO2AttestationResult(true, attestationPath);
            WriteObject(result);
        }

        /// <summary>
        /// Parses OpenSSH ssh-sk-attest-v01 (or v00) format and extracts the attestation certificate.
        /// Format: SSH wire encoding - uint32 (big-endian) length + data for each field.
        /// Fields: version string, attestation cert (DER), signature, authData (v01 only), reserved flags (uint32), reserved string.
        /// See fill_attestation_blob in OpenSSH ssh-sk.c
        /// </summary>
        private static X509Certificate2 ParseSshSkAttestation(byte[] data)
        {
            int offset = 0;

            // Read version string (sshbuf_put_cstring)
            byte[] version = ReadSshString(data, ref offset);
            string versionStr = Encoding.UTF8.GetString(version);
            if (versionStr != "ssh-sk-attest-v01" && versionStr != "ssh-sk-attest-v00")
            {
                throw new ArgumentException($"Unsupported attestation format: {versionStr}. Expected ssh-sk-attest-v01 or ssh-sk-attest-v00.");
            }

            // Read attestation certificate (sshbuf_put_string)
            byte[] certDer = ReadSshString(data, ref offset);
            if (certDer.Length == 0) throw new ArgumentException("Attestation certificate is empty.");

            try
            {
                return new X509Certificate2(certDer);
            }
            catch
            {
                throw new ArgumentException("Invalid attestation certificate data in file.");
            }
        }

        /// <summary>
        /// Reads an SSH wire-encoded string: uint32 (big-endian) length + that many bytes.
        /// </summary>
        private static byte[] ReadSshString(byte[] data, ref int offset)
        {
            if (offset + 4 > data.Length)
                throw new ArgumentException("Attestation file truncated or malformed.");

            int length = ((data[offset] & 0xFF) << 24) | ((data[offset + 1] & 0xFF) << 16)
                | ((data[offset + 2] & 0xFF) << 8) | (data[offset + 3] & 0xFF);
            offset += 4;

            if (length < 0 || offset + length > data.Length)
                throw new ArgumentException("Attestation file truncated or malformed.");

            byte[] value = new byte[length];
            if (length > 0)
                Array.Copy(data, offset, value, 0, length);
            offset += length;

            return value;
        }
    }
}
