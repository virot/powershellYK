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


namespace powershellYK.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.Set, "YubikeyPIV")]
    public class SetYubikeyPIVCommand : PSCmdlet
    {

        [Parameter(Mandatory = false, ParameterSetName = "ChangePIN", ValueFromPipeline = false, HelpMessage = "Easy access to ChangePIN")]
        public SwitchParameter ChangePIN;
        [Parameter(Mandatory = false, ParameterSetName = "ChangePUK", ValueFromPipeline = false, HelpMessage = "Easy access to ChangePUK")]
        public SwitchParameter ChangePUK;
        [Parameter(Mandatory = false, ParameterSetName = "UnblockPIN", ValueFromPipeline = false, HelpMessage = "Easy access to UnblockPIN")]
        public SwitchParameter UnblockPIN;


        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change number of PIN retries")]
        public byte? PinRetries { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ChangeRetries", ValueFromPipeline = false, HelpMessage = "Change number of PUK retries")]
        public byte? PukRetries { get; set; }

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



        protected override void BeginProcessing()
        {
            if (YubiKeyModule._pivSession is null || YubiKeyModule._pivSession.PinVerified == false)
            {
                WriteWarning("PIV not connected/authorized, Invoking Connect-YubikeyPIV");
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
        }

        protected override void ProcessRecord()
        {
            int? retriesLeft = null;

            WriteDebug($"Using ParameterSetName: {ParameterSetName}");
            switch (ParameterSetName)
            {
                case "ChangeRetries":
                    try
                    {
                        YubiKeyModule._pivSession!.ChangePinAndPukRetryCounts((byte)PinRetries!, (byte)PukRetries!);
                        WriteWarning("PIN and PUK codes reset to default, remember to change.");
                    }
                  
                    catch (Exception e)
                    {
                        throw new Exception("Failed to set PIN and PUK retries", e);
                    }
                    break;
                case "ChangePIN":
                    try
                    {
                        if (YubiKeyModule._pivSession!.TryChangePin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PIN))!)
, System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN!))!)
, out retriesLeft))
                        {
                            throw new Exception("Failed to reset PIN");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to change PIN", e);
                    }
                    finally
                    {
                    }
                    break;
                case "ChangePUK":
                    try
                    {
                        if (YubiKeyModule._pivSession!.TryChangePuk(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPUK))!)
, out retriesLeft))
                        {
                            throw new Exception("Failed to reset PIN");
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
                        if (YubiKeyModule._pivSession!.TryResetPin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!)
, System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN))!)
, out retriesLeft))
                        {
                            throw new Exception("Failed to reset PIN");
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