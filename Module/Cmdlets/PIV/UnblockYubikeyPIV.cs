/// <summary>
/// Unblocks a YubiKey PIV PIN using the PUK (PIN Unblocking Key).
/// Allows resetting a blocked PIN to a new value using the PUK.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// Unblock-YubiKeyPIV -PUK $puk -NewPIN $newPIN
/// Unblocks the PIV PIN using the PUK and sets a new PIN
/// 
/// .EXAMPLE
/// Unblock-YubiKeyPIV -PUK (ConvertTo-SecureString "12345678" -AsPlainText -Force) -NewPIN (ConvertTo-SecureString "87654321" -AsPlainText -Force)
/// Unblocks the PIV PIN using a PUK and sets a new PIN with explicit values
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
    [Cmdlet(VerbsSecurity.Unblock, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class UnblockYubikeyPIVCmdlet : Cmdlet
    {
        // Parameters for PIN unblocking
        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        public SecureString NewPIN { get; set; } = new SecureString();

        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        public SecureString PUK { get; set; } = new SecureString();

        // Connect to YubiKey when cmdlet starts
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

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Track remaining retry attempts for PIN operations
            int? retriesLeft = null;

            // Open a session with the YubiKey PIV application
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                // Set up key collector for PIN entry
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;

                try
                {
                    if (pivSession.TryResetPin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!)
, System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN))!)
, out retriesLeft) == false)
                    {
                        throw new Exception("Incorrect PUK provided.");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to reset PIN!", e);
                }
                finally
                {
                }
            }
        }
    }
}
