/// <summary>
/// Blocks the PIN and/or PUK of a YubiKey PIV application.
/// Can block either or both authentication methods by exhausting retry attempts.
/// Requires a YubiKey with PIV support.
/// 
/// .EXAMPLE
/// Block-YubiKeyPIV -PIN
/// Blocks the PIN by exhausting retry attempts
/// 
/// .EXAMPLE
/// Block-YubiKeyPIV -PIN -PUK
/// Blocks both PIN and PUK by exhausting retry attempts
/// </summary>

// Imports
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;

namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsSecurity.Block, "YubiKeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class BlockYubikeyPIVCommand : Cmdlet
    {
        // Parameter for blocking PIN
        [Parameter(Mandatory = true, ParameterSetName = "BlockBoth", ValueFromPipeline = false, HelpMessage = "Block the PIN for the PIV device")]
        [Parameter(Mandatory = true, ParameterSetName = "BlockPIN", ValueFromPipeline = false, HelpMessage = "Block the PIN for the PIV device")]
        public SwitchParameter PIN { get; set; }

        // Parameter for blocking PUK
        [Parameter(Mandatory = true, ParameterSetName = "BlockBoth", HelpMessage = "Block the PUK for the PIV device")]
        [Parameter(Mandatory = true, ParameterSetName = "BlockPUK", HelpMessage = "Block the PUK for the PIV device")]
        public SwitchParameter PUK { get; set; }

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
                // Block PIN if requested
                if (PIN.IsPresent)
                {
                    try
                    {
                        int? retriesRemaining = 1;
                        Random rnd = new Random();
                        while (retriesRemaining > 0)
                        {
                            int randomNumber = rnd.Next(0, 99999999);
                            string pinfail = randomNumber.ToString("00000000");
                            byte[] pinfailBytes = Encoding.UTF8.GetBytes(pinfail);
                            pivSession.TryChangePin(pinfailBytes, pinfailBytes, out retriesRemaining);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "There are no retries remaining for a PIN, PUK, or other authentication element.")
                        {
                            throw new Exception("Failed to block PUK!", e);
                        }
                    }
                }

                // Block PUK if requested
                if (PUK.IsPresent)
                {
                    try
                    {
                        int? retriesRemaining = 1;
                        Random rnd = new Random();
                        while (retriesRemaining > 0)
                        {
                            int randomNumber = rnd.Next(0, 99999999);
                            string pukfail = randomNumber.ToString("00000000");
                            byte[] pukfailBytes = Encoding.UTF8.GetBytes(pukfail);
                            pivSession.TryChangePuk(pukfailBytes, pukfailBytes, out retriesRemaining);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "There are no retries remaining for a PIN, PUK, or other authentication element.")
                        {
                            throw new Exception("Failed to block PUK!", e);
                        }
                    }
                }
            }
        }
    }
}
