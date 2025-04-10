    /// <summary>
    /// Performs Authenticode signing using a YubiKey PIV slot.
    /// This cmdlet signs executables and other signable files using the certificate stored in the specified PIV slot.
    /// Supports signing individual files or all signable files in a directory.
    /// Reports signature completion time in milliseconds for performance monitoring.
    /// Automatically uses SHA-256 as the hash algorithm for Authenticode signatures.
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
    /// # Sign all files in a file using a timestamp server
    /// New-YubiKeySignature -Slot "9c" -File "C:\CODE\CSRInspector\MSI\Release\CSRInspector.msi" -PIN "123456" -TimestampServer "http://timestamp.digicert.com"
    /// 
    /// .EXAMPLE
    /// # Sign all files in a directory with debugging
    /// New-YubiKeySignature -Slot "9c" -Dir "C:\CODE\CSRInspector\MSI\Release" -PIN "123456" -Debug
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
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace powershellYK.Cmdlets.PIV
{

    [Cmdlet(VerbsCommon.New, "YubiKeySignature")]
    public class NewYubiKeySignatureCommand : Cmdlet
    {
        [ArgumentCompletions("\"PIV Authentication\"", "\"Digital Signature\"", "\"Key Management\"", "\"Card Authentication\"", "0x9a", "0x9c", "0x9d", "0x9e")]
        [Parameter(Mandatory = true, ValueFromPipeline = false, HelpMessage = "PIV slot to use for signing")]
        public PIVSlot Slot { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "File to sign", ParameterSetName = "File")]
        public System.IO.FileInfo? File { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Directory containing files to sign", ParameterSetName = "Directory")]
        public System.IO.DirectoryInfo? Dir { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "PIN for the YubiKey")]
        public string? PIN { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Timestamp Server URL for signing. Uses free timestamping services provided by Certificate Authorities.")]
        [ArgumentCompletions(
            "http://timestamp.digicert.com",      // DigiCert - most reliable
            "http://timestamp.sectigo.com",       // Sectigo (formerly Comodo)
            "http://timestamp.globalsign.com/tsa/v3", // GlobalSign
            "http://time.certum.pl",              // Certum
            "http://timestamp.entrust.net/TSS/RFC3161" // Entrust
        )]
        public string? TimestampServer { get; set; }

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
            // Validate that only one parameter set is used
            if (File != null && Dir != null)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("You must specify either -File or -Dir, not both."),
                    "InvalidParameterSet",
                    ErrorCategory.InvalidArgument,
                    this));
            }

            // Warn about missing timestamp server
            if (string.IsNullOrEmpty(TimestampServer))
            {
                WriteWarning("No timestamp server provided. Signature may appear invalid after certificate expiration.");
            }

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

                // Handle directory signing mode
                if (Dir != null)
                {
                    WriteDebug($"Directory signing mode: {Dir.FullName}");
                    if (!Dir.Exists)
                    {
                        throw new Exception($"Directory {Dir.FullName} does not exist");
                    }

                    var allowedExtensions = new[] { 
                        "*.msi", "*.exe", "*.dll", "*.sys", 
                        "*.cat", "*.ocx", "*.cab", "*.ps1", 
                        "*.psm1", "*.vbs", "*.js" 
                    };

                    var filesToSign = allowedExtensions
                        .SelectMany(ext => Dir.GetFiles(ext))
                        .Distinct()
                        .ToArray();

                    WriteDebug($"Found {filesToSign.Length} files to sign");

                    object writeLock = new object();
                    var parallelOptions = new ParallelOptions 
                    { 
                        MaxDegreeOfParallelism = Math.Min(4, Environment.ProcessorCount) 
                    };

                    Parallel.ForEach(filesToSign, parallelOptions, fileInfo =>
                    {
                        try
                        {
                            byte[] fileData = System.IO.File.ReadAllBytes(fileInfo.FullName);
                            long elapsed = SignData(pivSession, publicKey, fileData);
                            
                            lock (writeLock)
                            {
                                WriteInformation(
                                    $"Successfully signed file: {fileInfo.FullName} ({elapsed}ms)", 
                                    new string[] { "SIGN" }
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (writeLock)
                            {
                                WriteWarning($"Skipping file {fileInfo.FullName} due to error: {ex.Message}");
                            }
                        }
                    });
                }
                
                // Handle single file signing mode
                else if (File != null)
                {
                    WriteDebug($"Single file signing mode: {File.FullName}");
                    if (!File.Exists)
                    {
                        throw new Exception($"File {File.FullName} does not exist");
                    }

                    byte[] fileData = System.IO.File.ReadAllBytes(File.FullName);
                    WriteDebug($"File size: {fileData.Length} bytes");
                    long elapsed = SignData(pivSession, publicKey, fileData);
                    WriteInformation($"Successfully signed file: {File.FullName} ({elapsed}ms)", new string[] { "SIGN" });
                }
            }
        }

        /// Signs the provided data using the YubiKey's PIV slot
        /// Returns the time taken to perform the signing operation in milliseconds
        private long SignData(PivSession pivSession, PivPublicKey publicKey, byte[] dataToSign)
        {
            WriteDebug($"Starting signing operation with {publicKey.GetType().Name}");
            var stopwatch = Stopwatch.StartNew();

            // Get the certificate from the YubiKey slot
            X509Certificate2? certificate = pivSession.GetCertificate(Slot);
            if (certificate == null)
            {
                throw new Exception($"No certificate found in slot {Slot}");
            }
            
            // Add detailed certificate information in debug messages
            WriteDebug($"Retrieved certificate from slot {Slot}:");
            WriteDebug($"  Subject: {certificate.Subject}");
            WriteDebug($"  Issuer: {certificate.Issuer}");
            WriteDebug($"  Valid from: {certificate.NotBefore} to {certificate.NotAfter}");
            WriteDebug($"  Has private key: {certificate.HasPrivateKey}");
            WriteDebug($"  Key algorithm: {certificate.GetKeyAlgorithm()}");
            WriteDebug($"  Key usage: {string.Join(", ", certificate.Extensions.OfType<X509KeyUsageExtension>().Select(x => x.KeyUsages.ToString()))}");
            //WriteDebug($"  Enhanced key usage: {string.Join(", ", certificate.Extensions.OfType<X509EnhancedKeyUsageExtension>().SelectMany(x => x.EnhancedKeyUsages.FriendlyName))}");
            WriteDebug($"  Certificate handle: 0x{certificate.Handle.ToInt64():X8}");

            try
            {
                var signer = new AuthenticodeSignature(this);
                WriteDebug($"Attempting to sign file: {File!.FullName}");
                WriteDebug($"Using timestamp server: {(string.IsNullOrEmpty(TimestampServer) ? "none" : TimestampServer)}");
                signer.SignFile(File!.FullName, certificate, TimestampServer);
                WriteDebug("Authenticode signing completed");
            }
            catch (Win32Exception ex)
            {
                WriteDebug($"Win32 error code: 0x{ex.NativeErrorCode:X8}");
                WriteDebug($"Win32 error message: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                WriteDebug($"Exception type: {ex.GetType().Name}");
                WriteDebug($"Exception message: {ex.Message}");
                WriteDebug($"Stack trace: {ex.StackTrace}");
                throw;
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private class AuthenticodeSignature
        {
            private readonly Cmdlet _cmdlet;

            public AuthenticodeSignature(Cmdlet cmdlet)
            {
                _cmdlet = cmdlet;
            }

            private void WriteDebug(string message)
            {
                _cmdlet.WriteDebug(message);
            }

            private class SafeSignerHandle : SafeHandle
            {
                public SafeSignerHandle() : base(IntPtr.Zero, true) { }

                public override bool IsInvalid => handle == IntPtr.Zero;

                protected override bool ReleaseHandle()
                {
                    if (!IsInvalid)
                    {
                        Marshal.FreeHGlobal(handle);
                    }
                    return true;
                }

                public void InitializeHandle(IntPtr newHandle)
                {
                    SetHandle(newHandle);
                }
            }

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

            public void SignFile(string filePath, X509Certificate2 certificate, string? timestampUrl = null)
            {
                WriteDebug($"SignFile called with path: {filePath}");
                WriteDebug($"Certificate info in SignFile:");
                WriteDebug($"  Handle: 0x{certificate.Handle.ToInt64():X8}");
                WriteDebug($"  Subject: {certificate.Subject}");

                if (certificate == null || certificate.Handle == IntPtr.Zero)
                {
                    throw new ArgumentException("Invalid certificate or certificate handle is null", nameof(certificate));
                }

                using var fileInfoHandle = new SafeSignerHandle();
                using var dwIndexHandle = new SafeSignerHandle();
                using var subjectInfoHandle = new SafeSignerHandle();
                using var signerCertHandle = new SafeSignerHandle();
                using var signatureInfoHandle = new SafeSignerHandle();

                try
                {
                    // Setup SIGNER_FILE_INFO
                    var fileInfo = new SIGNER_FILE_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_FILE_INFO)),
                        pwszFileName = filePath,
                        hFile = IntPtr.Zero
                    };
                    fileInfoHandle.InitializeHandle(Marshal.AllocHGlobal(Marshal.SizeOf(fileInfo)));
                    Marshal.StructureToPtr(fileInfo, fileInfoHandle.DangerousGetHandle(), false);

                    // Setup dwIndex
                    dwIndexHandle.InitializeHandle(Marshal.AllocHGlobal(sizeof(uint)));
                    Marshal.WriteInt32(dwIndexHandle.DangerousGetHandle(), 0);

                    // Setup SIGNER_SUBJECT_INFO
                    var subjectInfo = new SIGNER_SUBJECT_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SUBJECT_INFO)),
                        pdwIndex = dwIndexHandle.DangerousGetHandle(),
                        dwSubjectChoice = SIGNER_SUBJECT_FILE,
                        Union1 = new SubjectChoiceUnion
                        {
                            pSignerFileInfo = fileInfoHandle.DangerousGetHandle()
                        }
                    };
                    subjectInfoHandle.InitializeHandle(Marshal.AllocHGlobal(Marshal.SizeOf(subjectInfo)));
                    Marshal.StructureToPtr(subjectInfo, subjectInfoHandle.DangerousGetHandle(), false);

                    // Setup SIGNER_CERT
                    var signerCert = new SIGNER_CERT
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_CERT)),
                        dwCertChoice = 2, // SIGNER_CERT_SPC or SIGNER_CERT_CONTEXT
                        Union1 = new SignerCertUnion { pCertContext = certificate.Handle },
                        pwszStoreName = null,
                        pwszStoreLocation = null
                    };
                    signerCertHandle.InitializeHandle(Marshal.AllocHGlobal(Marshal.SizeOf(signerCert)));
                    Marshal.StructureToPtr(signerCert, signerCertHandle.DangerousGetHandle(), false);

                    // Setup SIGNER_SIGNATURE_INFO
                    var signatureInfo = new SIGNER_SIGNATURE_INFO
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SIGNER_SIGNATURE_INFO)),
                        algidHash = CALG_SHA_256,
                        dwAttrChoice = 0,
                        pAttrAuthCode = IntPtr.Zero,
                        pwszTimestampURL = timestampUrl
                    };
                    signatureInfoHandle.InitializeHandle(Marshal.AllocHGlobal(Marshal.SizeOf(signatureInfo)));
                    Marshal.StructureToPtr(signatureInfo, signatureInfoHandle.DangerousGetHandle(), false);

                    // Call SignerSign
                    WriteDebug("Calling SignerSign");
                    int result = SignerSign(
                        subjectInfoHandle.DangerousGetHandle(),
                        signerCertHandle.DangerousGetHandle(),
                        signatureInfoHandle.DangerousGetHandle(),
                        IntPtr.Zero);

                    WriteDebug($"SignerSign result: 0x{result:X8}");

                    if (result != 0)
                    {
                        throw new Win32Exception(result, "SignerSign failed");
                    }
                }
                catch (Win32Exception ex)
                {
                    WriteDebug($"Win32Exception in SignFile:");
                    WriteDebug($"  NativeErrorCode: 0x{ex.NativeErrorCode:X8}");
                    WriteDebug($"  Message: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 
