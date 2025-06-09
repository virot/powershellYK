/// <summary>
/// Configures various PIV settings on a YubiKey, including PIN, PUK, management key, and retry counts.
/// Supports changing PIN/PUK, unblocking PIN, modifying retry counts, and generating new CHUID.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// Set-YubiKeyPIV -ChangePIN -PIN $currentPIN -NewPIN $newPIN
/// Changes the PIV PIN
/// 
/// .EXAMPLE
/// Set-YubiKeyPIV -ChangeRetries -PinRetries 3 -PukRetries 3
/// Sets the number of PIN and PUK retry attempts
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubico.YubiKey.Piv.Objects;
using powershellYK.support;
using System.Security;
using System.Runtime.InteropServices;
using powershellYK.support.transform;
using powershellYK.support.validators;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Set, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class SetYubikeyPIVCommand : PSCmdlet
    {
        // Parameters for PIN/PUK operations
        [Parameter(Mandatory = false, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "Change the PIN")]
        public SwitchParameter ChangePIN;

        [Parameter(Mandatory = false, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "Change the PUK (PIN Unblocking Key)")]
        public SwitchParameter ChangePUK;

        [Parameter(Mandatory = false, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Unblock the PIN")]
        public SwitchParameter UnblockPIN;

        // Parameters for retry count configuration
        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change the number of PIN retries")]
        public byte? PinRetries { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change the number of PUK retries")]
        public byte? PukRetries { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Keep PUK unlocked", DontShow = true)]
        public SwitchParameter KeepPukUnlocked { get; set; }

        // Parameters for PIN/PUK values
        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "Current PIN")]
        public SecureString PIN { get; set; } = new SecureString();

        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        public SecureString NewPIN { get; set; } = new SecureString();

        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        public SecureString PUK { get; set; } = new SecureString();

        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "New PUK")]
        public SecureString NewPUK { get; set; } = new SecureString();

        // Parameters for management key operations
        [TransformHexInput()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Current Management Key")]
        public PSObject ManagementKey { get; set; } = new PSObject();

        [TransformHexInput()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "New Management Key")]
        public PSObject NewManagementKey { get; set; } = new PSObject();

        [ValidateSet("TripleDES", "AES128", "AES192", "AES256", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; } = PivAlgorithm.TripleDes;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Touch policy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        // Parameters for other PIV operations
        [Parameter(Mandatory = true, ParameterSetName = "newCHUID", ValueFromPipeline = false, HelpMessage = "Generate new CHUID")]
        public SwitchParameter newCHUID { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Set Managementkey to PIN protected", ValueFromPipeline = false, HelpMessage = "PIN protect the Management Key")]
        public SwitchParameter PINProtectedManagementkey { get; set; }

        // Called when the cmdlet begins processing
        protected override void BeginProcessing()
        {
            // Check if a YubiKey is connected, if not attempt to connect
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No YubiKey selected, calling Connect-Yubikey...");
                try
                {
                    // Create a new PowerShell instance to run Connect-Yubikey
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

        // Main logic for PIV operations
        protected override void ProcessRecord()
        {
            // Track remaining retry attempts for PIN/PUK operations
            int? retriesLeft = null;

            // Open a session with the YubiKey PIV application
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN entry
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                WriteDebug($"Using ParameterSetName: {ParameterSetName}");

                // Handle different parameter sets
                switch (ParameterSetName)
                {
                    case "ChangeRetries":
                        // Verify device supports retry count changes
                        if (new List<FormFactor> { FormFactor.UsbABiometricKeychain, FormFactor.UsbCBiometricKeychain }.Contains(((YubiKeyDevice)YubiKeyModule._yubikey!).FormFactor))
                        {
                            throw new Exception("Biometric YubiKeys does not support changing the number of PIN retries.");
                        }
                        // powershellYK does more than the SDK here, it also blocks the PUK if the Management key is PIN protected.
                        pivSession.ChangePinAndPukRetryCounts((byte)PinRetries!, (byte)PukRetries!);

                        // Yubikey disables the PUK if the Management key is PIN protected, we do the same if not KeepPUKUnlocked is set
                        if (pivSession.GetPinOnlyMode().HasFlag(PivPinOnlyMode.PinProtected) && !(KeepPukUnlocked.IsPresent))
                        {
                            WriteDebug("Management Key is PIN protected, Blocking PUK...");
                            retriesLeft = 1;

                            // Block PUK by setting it to a known value
                            while (retriesLeft > 0)
                            {
                                pivSession.TryChangePuk(System.Text.Encoding.UTF8.GetBytes("87654321"), System.Text.Encoding.UTF8.GetBytes("23456789"), out retriesLeft);
                            }

                            // Restore original PIN if available
                            if (YubiKeyModule._pivPIN is not null && YubiKeyModule._pivPIN.Length > 0 && Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!)) != "123456")
                            {
                                WriteDebug("Trying to revert PIN...");

                                // Reset PIN to original value
                                pivSession.TryChangePin(System.Text.Encoding.UTF8.GetBytes("123456"), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!))!), out retriesLeft);
                            }
                            else
                            {
                                WriteWarning("PIN not set, remember to set it!");
                            }
                        }
                        else
                        {
                            WriteWarning("PIN and PUK codes reset to default, remember to change them!");
                        }

                        WriteInformation($"Number of PIN/PUK retries set ({PinRetries},{PukRetries}).", new string[] { "PIV", "Info" });
                        break;

                    case "ChangePIN":
                        try
                        {
                            if (pivSession.TryChangePin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PIN))!)
    , System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN!))!)
    , out retriesLeft) == true)
                            {
                                // Update stored PIN in module
                                YubiKeyModule._pivPIN = NewPIN;
                                WriteInformation("PIN updated.", new string[] { "PIV", "Info" });
                            }
                            else
                            {
                                throw new Exception("Incorrect PIN provided.");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to change PIN.", e);
                        }
                        finally
                        {
                        }
                        break;

                    case "ChangePUK":
                        try
                        {
                            if (pivSession.TryChangePuk(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPUK))!)
    , out retriesLeft) == false)
                            {
                                throw new Exception("Incorrect PUK provided.");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to change PUK", e);
                        }
                        finally
                        {
                        }
                        break;

                    case "ResetPIN":
                        try
                        {
                            if (pivSession.TryResetPin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!)
    , System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN))!)
    , out retriesLeft) == false)
                            {
                                throw new Exception("Incorrect PUK provided");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to reset PIN", e);
                        }
                        finally
                        {
                        }
                        break;
                    case "ChangeManagement":

                        // Convert management keys from PSObject to byte arrays
                        byte[] ManagementKeyarray = (byte[])ManagementKey.BaseObject;
                        byte[] NewManagementKeyarray = (byte[])NewManagementKey.BaseObject;
                        try
                        {
                            // Change management key with new algorithm and touch policy
                            if (pivSession.TryChangeManagementKey(ManagementKeyarray, NewManagementKeyarray, (PivTouchPolicy)TouchPolicy, (PivAlgorithm)Algorithm))
                            {
                                WriteDebug("Management Key changed");
                            }
                            else
                            {
                                throw new Exception("Failed to change Management Key");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to change Management Key", e);
                        }
                        finally
                        {
                            // Clear sensitive data from memory
                            CryptographicOperations.ZeroMemory(ManagementKeyarray);
                            CryptographicOperations.ZeroMemory(NewManagementKeyarray);
                        }
                        break;

                    case "newCHUID":

                        // Generate and write new CHUID
                        CardholderUniqueId chuid = new CardholderUniqueId();
                        chuid.SetRandomGuid();
                        try
                        {
                            // Write new CHUID to YubiKey
                            pivSession.WriteObject(chuid);
                            WriteInformation("New CHUID set.", new string[] { "PIV", "Info" });
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to generate new CHUID", e);
                        }
                        break;

                    case "Set Managementkey to PIN protected":

                        // Determine appropriate algorithm based on YubiKey capabilities
                        PivAlgorithm mgmtKeyAlgorithm = ((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivAesManagementKey) ? PivAlgorithm.Aes256 : PivAlgorithm.TripleDes;

                        // Set management key to be PIN protected
                        pivSession.SetPinOnlyMode(PivPinOnlyMode.PinProtected, mgmtKeyAlgorithm);
                        WriteInformation($"Management key set to PIN protected.", new string[] { "PIV", "Info" });
                        break;
                }
            }
        }
    }
}
