using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using Yubico.YubiKey.Piv.Objects;
using powershellYK.support;


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Set, "YubikeyPIV")]
    public class SetYubikeyPIVCommand : PSCmdlet
    {

        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change number of PIN retries")]
        public byte? PinRetries { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change number of PUK retries")]
        public byte? PukRetries { get; set; }

        [ValidateLength(6, 8)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "Current PIN")]
        public string? PIN { get; set; }

        [ValidateLength(6, 8)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "New PIN")]
        public string? NewPIN { get; set; }

        [ValidateLength(6, 8)]
        [Parameter(Mandatory = true, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "Current PUK")]
        public string? PUK { get; set; }

        [ValidateLength(6, 8)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "New PUK")]
        public string? NewPUK { get; set; }

        [ValidateLength(48, 48)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Current ManagementKey")]
        public string? ManagementKey { get; set; }

        [ValidateLength(48, 48)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "New ManagementKey")]
        public string? NewManagementKey { get; set; }

        [ValidateSet("TripleDES", "AES128", "AES192", "AES256", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; } = PivAlgorithm.TripleDes;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [Parameter(Mandatory = true, ParameterSetName = "newCHUID", ValueFromPipeline = false, HelpMessage = "Generate new CHUID")]
        public SwitchParameter newCHUID { get; set; }


        protected override void ProcessRecord()
        {
            if (YubiKeyModule._pivSession is null)
            {
                //throw new Exception("PIV not connected, use Connect-YubikeyPIV first");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-YubikeyPIV");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }

            byte[] pinarray;
            byte[] newpinarray;
            byte[] pukarray;
            byte[] newpukarray;
            int? retriesLeft = null;


            WriteDebug($"Using ParameterSetName: {ParameterSetName}");
            switch (ParameterSetName)
            {
                case "ChangeRetries":
                    try
                    {
                        YubiKeyModule._pivSession!.ChangePinAndPukRetryCounts((byte)PinRetries, (byte)PukRetries);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to set PIN and PUK retries", e);
                    }
                    break;
                case "ChangePIN":
                    pinarray = System.Text.Encoding.UTF8.GetBytes(PIN!);
                    newpinarray = System.Text.Encoding.UTF8.GetBytes(NewPIN!);
                    try
                    {
                        YubiKeyModule._pivSession!.TryChangePin(pinarray, newpinarray, out retriesLeft);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to change PIN", e);
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(pinarray);
                        CryptographicOperations.ZeroMemory(newpinarray);
                    }
                    break;
                case "ChangePUK":
                    pukarray = System.Text.Encoding.UTF8.GetBytes(PUK!);
                    newpukarray = System.Text.Encoding.UTF8.GetBytes(NewPUK!);
                    try
                    {
                        YubiKeyModule._pivSession!.TryChangePuk(pukarray, newpukarray, out retriesLeft);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to change PUK", e);
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(pukarray);
                        CryptographicOperations.ZeroMemory(newpukarray);
                    }
                    break;
                case "ResetPIN":
                    pukarray = System.Text.Encoding.UTF8.GetBytes(PUK!);
                    newpinarray = System.Text.Encoding.UTF8.GetBytes(NewPIN!);
                    try
                    {
                        YubiKeyModule._pivSession!.TryResetPin(pukarray, newpinarray, out retriesLeft);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to reset PIN", e);
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(pukarray);
                        CryptographicOperations.ZeroMemory(newpinarray);
                    }
                    break;
                case "ChangeManagement":
                    byte[] ManagementKeyarray = HexConverter.StringToByteArray(ManagementKey!);
                    byte[] NewManagementKeyarray = HexConverter.StringToByteArray(NewManagementKey!);
                    try
                    {
                        if (YubiKeyModule._pivSession!.TryChangeManagementKey(ManagementKeyarray, NewManagementKeyarray, (PivTouchPolicy)TouchPolicy, (PivAlgorithm)Algorithm))
                        {
                            WriteDebug("ManagementKey changed");
                        }
                        else
                        {
                            throw new Exception("Failed to change ManagementKey");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to change ManagementKey", e);
                    }
                    finally
                    {
                        CryptographicOperations.ZeroMemory(ManagementKeyarray);
                        CryptographicOperations.ZeroMemory(NewManagementKeyarray);
                    }
                    break;
                case "newCHUID":
                    CardholderUniqueId chuid = new CardholderUniqueId();
                    chuid.SetRandomGuid();
                    try
                    {
                        YubiKeyModule._pivSession!.WriteObject(chuid);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to generate new CHUID", e);
                    }
                    break;
            }
        }
    }
}