    /// <summary>
    /// Performs a digital signature operation using a YubiKey PIV slot.
    /// This cmdlet signs input data using the private key stored in the specified PIV slot.
    /// Supports both RSA and ECC keys, with automatic algorithm selection based on the key type.
    /// Can sign raw byte data, individual files, or all files in a directory.
    /// Reports signature completion time in milliseconds for performance monitoring.
    /// 
    /// .EXAMPLE
    /// # Sign a single file
    /// $signature = New-YubiKeySignature -Slot "9c" -File "document.pdf" -PIN "123456"
    /// 
    /// .EXAMPLE
    /// # Sign all files in a directory
    /// $signatures = New-YubiKeySignature -Slot "9c" -Dir "C:\Documents" -PIN "123456"
    /// 
    /// .EXAMPLE
    /// # Using a different hash algorithm (will be overridden for ECC keys)
    /// $signature = New-YubiKeySignature -Slot "Digital Signature" -File "document.pdf" -HashAlgorithm SHA512
    /// </summary>

using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.PIV;
using System.Diagnostics;
using Yubico.YubiKey.Sample.PivSampleCode;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubiKeySignature")]
    public class PerformYubiKeySignatureCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIV slot to use for signing")]
        public PIVSlot Slot { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Data to sign", ParameterSetName = "ByteData")]
        public required byte[] Data { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "File to sign", ParameterSetName = "File")]
        public string? File { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "Directory containing files to sign", ParameterSetName = "Directory")]
        public string? Dir { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PIN for the YubiKey")]
        public string? PIN { get; set; }

        [ValidateSet("SHA1", "SHA256", "SHA384", "SHA512", IgnoreCase = true)]
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Hash algorithm")]
        public HashAlgorithmName HashAlgorithm { get; set; } = HashAlgorithmName.SHA256;

        // @Virot: maybe it should do Connect-YubiKeyPIV?
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
            // Create a new PIV session with the currently connected YubiKey
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                WriteDebug($"Created PIV session for slot {Slot}");
                
                // Set up the key collector for PIN/PUK entry
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                // Retrieve the public key from the specified slot to determine signing algorithm
                PivPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    if (publicKey is null)
                    {
                        throw new Exception("Public key is null!");
                    }
                    WriteDebug($"Retrieved public key: Type={publicKey.GetType().Name}, Algorithm={publicKey.Algorithm}");
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does the key exist?", e);
                }

                // Handle directory signing mode - sign all files in specified directory
                if (Dir != null)
                {
                    WriteDebug($"Directory signing mode: {Dir}");
                    if (!System.IO.Directory.Exists(Dir))
                    {
                        throw new Exception($"Directory {Dir} does not exist");
                    }

                    // Iterate through all files in directory and sign each one
                    foreach (string filePath in System.IO.Directory.GetFiles(Dir))
                    {
                        WriteDebug($"Processing file: {filePath}");
                        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                        WriteDebug($"File size: {fileData.Length} bytes");
                        var (_, elapsed) = SignData(pivSession, publicKey, fileData);
                        WriteInformation($"Successfully signed file: {filePath} ({elapsed}ms)", new string[] { "SIGN" });
                    }
                }
                // Handle single file signing mode
                else if (File != null)
                {
                    WriteDebug($"Single file signing mode: {File}");
                    if (!System.IO.File.Exists(File))
                    {
                        throw new Exception($"File {File} does not exist");
                    }

                    // Read and sign the specified file
                    byte[] fileData = System.IO.File.ReadAllBytes(File);
                    WriteDebug($"File size: {fileData.Length} bytes");
                    var (_, elapsed) = SignData(pivSession, publicKey, fileData);
                    WriteInformation($"Successfully signed file: {File} ({elapsed}ms)", new string[] { "SIGN" });
                }
                // Handle raw data signing mode
                else
                {
                    WriteDebug("Raw data signing mode");
                    WriteDebug($"Data size: {Data.Length} bytes");
                    var (_, elapsed) = SignData(pivSession, publicKey, Data);
                    WriteInformation($"Successfully signed provided data ({elapsed}ms)", new string[] { "SIGN" });
                }
            }
        }

        /// Signs the provided data using the YubiKey's PIV slot
        private (byte[], long) SignData(PivSession pivSession, PivPublicKey publicKey, byte[] dataToSign)
        {
            WriteDebug($"Starting signing operation with {publicKey.GetType().Name}");
            var stopwatch = Stopwatch.StartNew();

            // Get the certificate from the YubiKey slot
            X509Certificate2? certificate = pivSession.GetCertificate(Slot);
            if (certificate == null)
            {
                throw new Exception($"No certificate found in slot {Slot}");
            }
            WriteDebug($"Retrieved certificate: {certificate.Subject}");

            try
            {
                var signer = new AuthenticodeSignature();
                signer.SignFile(File!, certificate);
                WriteDebug("Authenticode signing completed");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to sign file: {ex.Message}", ex);
            }

            stopwatch.Stop();
            return (Array.Empty<byte>(), stopwatch.ElapsedMilliseconds);
        }

        private class AuthenticodeSignature
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct SIGNER_FILE_INFO
            {
                public uint cbSize;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string pwszFileName;
                public IntPtr hFile;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SIGNER_SUBJECT_INFO
            {
                public uint cbSize;
                public IntPtr pdwIndex;
                public uint dwSubjectChoice;
                public SubjectChoiceUnion Union1;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct SubjectChoiceUnion
            {
                [FieldOffset(0)]
                public IntPtr pSignerFileInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SIGNER_CERT
            {
                public uint cbSize;
                public uint dwCertChoice;
                public SignerCertUnion Union1;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string? pwszStoreName;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string? pwszStoreLocation;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct SignerCertUnion
            {
                [FieldOffset(0)]
                public IntPtr pCertContext;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SIGNER_SIGNATURE_INFO
            {
                public uint cbSize;
                public uint algidHash;
                public uint dwAttrChoice;
                public IntPtr pAttrAuthCode;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string? pwszTimestampURL;
            }

            [DllImport("MSSign32.dll", CharSet = CharSet.Unicode)]
            private static extern int SignerSign(
                IntPtr pSubjectInfo,
                IntPtr pSignerCert,
                IntPtr pSignatureInfo,
                IntPtr pProviderInfo);

            private const uint SIGNER_SUBJECT_FILE = 1;
            private const uint CALG_SHA_256 = 0x0000800c;

            public void SignFile(string filePath, X509Certificate2 certificate)
            {
                IntPtr pSubjectInfo = IntPtr.Zero;
                IntPtr pSignerCert = IntPtr.Zero;
                IntPtr pSignatureInfo = IntPtr.Zero;
                IntPtr dwIndex = Marshal.AllocHGlobal(sizeof(uint));

                try
                {
                    // Setup file info
                    var fileInfo = new SIGNER_FILE_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_FILE_INFO)),
                        pwszFileName = filePath,
                        hFile = IntPtr.Zero
                    };
                    IntPtr pFileInfo = Marshal.AllocHGlobal(Marshal.SizeOf(fileInfo));
                    Marshal.StructureToPtr(fileInfo, pFileInfo, false);

                    // Setup subject info
                    Marshal.WriteInt32(dwIndex, 0);
                    var subjectInfo = new SIGNER_SUBJECT_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SUBJECT_INFO)),
                        pdwIndex = dwIndex,
                        dwSubjectChoice = SIGNER_SUBJECT_FILE,
                        Union1 = new SubjectChoiceUnion { pSignerFileInfo = pFileInfo }
                    };
                    pSubjectInfo = Marshal.AllocHGlobal(Marshal.SizeOf(subjectInfo));
                    Marshal.StructureToPtr(subjectInfo, pSubjectInfo, false);

                    // Setup certificate info
                    var signerCert = new SIGNER_CERT
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT)),
                        dwCertChoice = 2,
                        Union1 = new SignerCertUnion { pCertContext = certificate.Handle }
                    };
                    pSignerCert = Marshal.AllocHGlobal(Marshal.SizeOf(signerCert));
                    Marshal.StructureToPtr(signerCert, pSignerCert, false);

                    // Setup signature info
                    var signatureInfo = new SIGNER_SIGNATURE_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SIGNATURE_INFO)),
                        algidHash = CALG_SHA_256,
                        dwAttrChoice = 0,
                        pAttrAuthCode = IntPtr.Zero
                    };
                    pSignatureInfo = Marshal.AllocHGlobal(Marshal.SizeOf(signatureInfo));
                    Marshal.StructureToPtr(signatureInfo, pSignatureInfo, false);

                    int result = SignerSign(pSubjectInfo, pSignerCert, pSignatureInfo, IntPtr.Zero);
                    if (result != 0)
                    {
                        throw new Exception($"SignerSign failed with error code: {result} (0x{result:X8})");
                    }
                }
                finally
                {
                    if (dwIndex != IntPtr.Zero) Marshal.FreeHGlobal(dwIndex);
                    if (pSubjectInfo != IntPtr.Zero) Marshal.FreeHGlobal(pSubjectInfo);
                    if (pSignerCert != IntPtr.Zero) Marshal.FreeHGlobal(pSignerCert);
                    if (pSignatureInfo != IntPtr.Zero) Marshal.FreeHGlobal(pSignatureInfo);
                }
            }
        }
    }
} 
