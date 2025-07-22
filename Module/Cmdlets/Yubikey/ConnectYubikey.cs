/// <summary>
/// Connects to a YubiKey for use with the powershellYK cmdlets.
/// Supports connecting to a specific YubiKey by device object or serial number,
/// or automatically connects to the only YubiKey if exactly one is present.
/// 
/// .EXAMPLE
/// Connect-YubiKey
/// Connects to the only YubiKey if exactly one is present
/// 
/// .EXAMPLE
/// Connect-YubiKey -Serialnumber 1234567
/// Connects to a specific YubiKey by serial number
/// 
/// .EXAMPLE
/// $YubiKey = Find-YubiKey | Select-Object -First 1
/// Connect-YubiKey -YubiKey $YubiKey
/// Connects to a specific YubiKey by device object
/// </summary>

using powershellYK.support;
using System.Diagnostics;
using System.Management.Automation;           // Windows PowerShell namespace.
using System.Runtime.InteropServices;
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommunications.Connect, "YubiKey", DefaultParameterSetName = "Connect single Yubikey")]
    public class ConnectYubikeyCommand : PSCmdlet
    {
        // Parameters for YubiKey selection
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = true, HelpMessage = "Which YubiKey to connect to", ParameterSetName = "Connect provided Yubikey")]
        public YubiKeyDevice? YubiKey { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Connect to YubiKey with Serial Number", ParameterSetName = "Connect Yubikey with Serialnumber")]
        public int? Serialnumber { get; set; }

        // Private field for YubiKey
        private YubiKeyDevice? _yubikey;

        // Initialize processing
        protected override void BeginProcessing()
        {
        }

        // Process the main cmdlet logic
        protected override void ProcessRecord()
        {
            // Disconnect from previous YubiKey if connected
            if (YubiKeyModule._yubikey is not null)
            {
                WriteDebug("Disconnecting from previous YubiKey");
                var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Disconnect-Yubikey");
                if (this.MyInvocation.BoundParameters.ContainsKey("InformationAction"))
                {
                    myPowersShellInstance = myPowersShellInstance.AddParameter("InformationAction", this.MyInvocation.BoundParameters["InformationAction"]);
                }
                myPowersShellInstance.Invoke();
            }

            WriteDebug("Starting the real connect part");
            switch (ParameterSetName)
            {
                case "Connect provided Yubikey":
                    // Connect to provided YubiKey
                    _yubikey = YubiKey;
                    break;

                case "Connect single Yubikey":
                    // Connect to the only YubiKey if exactly one is present
                    var yubikeys = YubiKeyDevice.FindAll();
                    if (yubikeys.Count() == 1)
                    {
                        _yubikey = (YubiKeyDevice)yubikeys.First();
                        WriteDebug($"Found only one device, using {_yubikey.SerialNumber.ToString() ?? "N/A"}.");
                    }
                    break;

                case "Connect Yubikey with Serialnumber":
                    // Connect to YubiKey with specific serial number
                    WriteDebug($"Looking for YubiKey with serial: {Serialnumber}.");
                    IYubiKeyDevice tempYubiKey;
                    if (YubiKeyDevice.TryGetYubiKey((int)Serialnumber!, out tempYubiKey))
                    {
                        _yubikey = (YubiKeyDevice)tempYubiKey;
                    }
                    else
                    {
                        throw new Exception($"The specific YubiKey ({Serialnumber}) was not found.");
                    }
                    break;

                default:
                    throw new Exception("Invalid ParameterSetName");
            }

            // Handle connection result
            if (_yubikey is not null)
            {
                // Store connected YubiKey in module
                YubiKeyModule._yubikey = _yubikey;
                if (_yubikey.SerialNumber is not null)
                {
                    WriteInformation($"Connected to {PowershellYKText.FriendlyName(_yubikey)} with serial: {_yubikey.SerialNumber}.", new string[] { "YubiKey" });
                }
                else
                {
                    WriteInformation($"Connected to {PowershellYKText.FriendlyName(_yubikey)} with serial: N/A.", new string[] { "YubiKey" });
                }
            }
            else
            {
                // Handle connection failure
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Windows.IsRunningAsAdministrator())
                {
                    WriteWarning("No YubiKeys found, FIDO-only YubiKeys on Windows requires running as Administrator.");
                }
                WriteError(new ErrorRecord(new Exception("None or multiple YubiKeys found, Use Connect-Yubikey to specify which Yubikey to use."), "0x00010001", ErrorCategory.InvalidResult, null));
            }
        }
    }
}
