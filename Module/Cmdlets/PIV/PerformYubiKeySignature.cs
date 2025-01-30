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
            WriteDebug($"Initial hash algorithm: {HashAlgorithm}");
            
            var stopwatch = Stopwatch.StartNew();
            byte[] signature;

            if (publicKey is PivRsaPublicKey rsaKey)
            {
                int keySize = publicKey.Algorithm switch
                {
                    PivAlgorithm.Rsa4096 => 4096,
                    PivAlgorithm.Rsa3072 => 3072,
                    PivAlgorithm.Rsa2048 => 2048,
                    PivAlgorithm.Rsa1024 => 1024,
                    _ => throw new Exception("Unsupported RSA key size")
                };
                WriteDebug($"RSA key detected: {keySize} bits");
                WriteDebug("Using PSS padding mode");
                // For RSA keys, use PSS padding mode for enhanced security
                var signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                signature = signer.SignData(dataToSign, HashAlgorithm);
            }
            else
            {
                WriteDebug($"ECC key detected: {publicKey.Algorithm}");
                // For ECC keys, hash algorithm must match the curve size
                HashAlgorithm = publicKey.Algorithm switch
                {
                    PivAlgorithm.EccP256 => HashAlgorithmName.SHA256,  // P-256 requires SHA256
                    PivAlgorithm.EccP384 => HashAlgorithmName.SHA384,  // P-384 requires SHA384
                    _ => throw new Exception("Unknown public Key algorithm")
                };
                
                WriteDebug($"Selected hash algorithm for ECC: {HashAlgorithm}");
                var signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                signature = signer.SignData(dataToSign, HashAlgorithm);
            }

            stopwatch.Stop();
            WriteDebug($"Signing completed in {stopwatch.ElapsedMilliseconds}ms");
            return (signature, stopwatch.ElapsedMilliseconds);
        }
    }
} 
