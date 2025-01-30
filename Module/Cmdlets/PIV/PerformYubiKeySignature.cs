    /// <summary>
    /// Performs a digital signature operation using a YubiKey PIV slot.
    /// This cmdlet signs input data using the private key stored in the specified PIV slot.
    /// Supports both RSA and ECC keys, with automatic algorithm selection based on the key type.
    /// Can sign raw byte data, individual files, or all files in a directory.
    /// Reports signature completion time in milliseconds for performance monitoring.
    /// 
    /// .EXAMPLE
    /// # Sign raw byte data
    /// $data = [System.Text.Encoding]::UTF8.GetBytes("Hello World")
    /// $signature = New-YubiKeySignature -Slot "9c" -Data $data -PIN "123456"
    /// # Output includes: "Successfully signed provided data (123ms)"
    /// 
    /// .EXAMPLE
    /// # Sign a single file
    /// $signature = New-YubiKeySignature -Slot "9c" -File "document.pdf" -PIN "123456"
    /// # Output includes: "Successfully signed file: document.pdf (234ms)"
    /// 
    /// .EXAMPLE
    /// # Sign all files in a directory
    /// $signatures = New-YubiKeySignature -Slot "9c" -Dir "C:\Documents" -PIN "123456"
    /// # Output includes timing for each file: "Successfully signed file: file1.pdf (156ms)"
    /// 
    /// .EXAMPLE
    /// # Using a different hash algorithm (will be overridden for ECC keys)
    /// $signature = New-YubiKeySignature -Slot "Digital Signature" -File "document.pdf" -HashAlgorithm SHA512
    /// </summary>
    /// 
    /// 

using System.Management.Automation;
using System.Security.Cryptography;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using powershellYK.PIV;
using System.Diagnostics;
//using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Sample.PivSampleCode;
//using powershellYK.support.transform;


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

                                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;


                // Check the public key to determine use of algorithm
                PivPublicKey? publicKey = null;
                try
                {
                    publicKey = pivSession.GetMetadata(Slot).PublicKey;
                    if (publicKey is null)
                    {
                        throw new Exception("Public key is null!");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to get public key for slot {Slot}, does the key exist?", e);
                }

                if (Dir != null)
                {
                    if (!System.IO.Directory.Exists(Dir))
                    {
                        throw new Exception($"Directory {Dir} does not exist");
                    }

                    foreach (string filePath in System.IO.Directory.GetFiles(Dir))
                    {
                        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                        var (signature, elapsed) = SignData(pivSession, publicKey, fileData);
                        WriteObject(new { FilePath = filePath, Signature = signature });
                        WriteInformation($"Successfully signed file: {filePath} ({elapsed}ms)", new string[] { "SIGN" });
                    }
                }
                else if (File != null)
                {
                    if (!System.IO.File.Exists(File))
                    {
                        throw new Exception($"File {File} does not exist");
                    }

                    byte[] fileData = System.IO.File.ReadAllBytes(File);
                    var (signature, elapsed) = SignData(pivSession, publicKey, fileData);
                    WriteObject(signature);
                    WriteInformation($"Successfully signed file: {File} ({elapsed}ms)", new string[] { "SIGN" });
                }
                else
                {
                    var (signature, elapsed) = SignData(pivSession, publicKey, Data);
                    WriteObject(signature);
                    WriteInformation($"Successfully signed provided data ({elapsed}ms)", new string[] { "SIGN" });
                }
            }
        }

        private (byte[], long) SignData(PivSession pivSession, PivPublicKey publicKey, byte[] dataToSign)
        {
            var stopwatch = Stopwatch.StartNew();
            byte[] signature;

            if (publicKey is PivRsaPublicKey)
            {
                // Create RSA signature generator with PSS padding
                var signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey, RSASignaturePaddingMode.Pss);
                signature = signer.SignData(dataToSign, HashAlgorithm);
            }
            else
            {
                // ECC signing with appropriate hash algorithm based on curve size
                HashAlgorithm = publicKey.Algorithm switch
                {
                    PivAlgorithm.EccP256 => HashAlgorithmName.SHA256,
                    PivAlgorithm.EccP384 => HashAlgorithmName.SHA384,
                    _ => throw new Exception("Unknown public Key algorithm")
                };
                
                var signer = new YubiKeySignatureGenerator(pivSession, Slot, publicKey);
                signature = signer.SignData(dataToSign, HashAlgorithm);
            }

            stopwatch.Stop();
            return (signature, stopwatch.ElapsedMilliseconds);
        }
    }
} 
