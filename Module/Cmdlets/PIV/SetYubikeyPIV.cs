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
    [Cmdlet(VerbsCommon.Set, "YubikeyPIV", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
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

        [TransformPivManagementKey()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Current ManagementKey")]
        public PSObject ManagementKey { get; set; } = new PSObject();

        [TransformPivManagementKey()]
        [ValidatePIVManagementKey()]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "New ManagementKey")]
        public PSObject NewManagementKey { get; set; } = new PSObject();

        [ValidateSet("TripleDES", "AES128", "AES192", "AES256", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; } = PivAlgorithm.TripleDes;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Mandatory = true, ParameterSetName = "ChangeManagement", ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

        [Parameter(Mandatory = true, ParameterSetName = "newCHUID", ValueFromPipeline = false, HelpMessage = "Generate new CHUID")]
        public SwitchParameter newCHUID { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "Set Managementkey to PIN protected", ValueFromPipeline = false, HelpMessage = "PIN protect the Managementkey")]
        public SwitchParameter PINProtectedManagementkey { get; set; }



        protected override void BeginProcessing()
        {
            if (YubiKeyModule._yubikey is null)
            {
                WriteDebug("No Yubikey selected, calling Connect-Yubikey");
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                    WriteDebug($"Successfully connected");
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
            using (var pivSession = new PivSession((YubiKeyDevice)YubiKeyModule._yubikey!))
            {
                pivSession.KeyCollector = YubiKeyModule._KeyCollector.YKKeyCollectorDelegate;
                WriteDebug($"Using ParameterSetName: {ParameterSetName}");
                switch (ParameterSetName)
                {
                    case "ChangeRetries":
                        // powershellYK does more than the SDK here, it also blocks the PUK if 
                        pivSession.ChangePinAndPukRetryCounts((byte)PinRetries!, (byte)PukRetries!);
                        //WriteWarning("PIN and PUK codes reset to default, remember to change.");
                        if (pivSession.GetPinOnlyMode().HasFlag(PivPinOnlyMode.PinProtected))
                        {
                            WriteDebug("Management key is PIN Protected, Blocking PUK");
                            retriesLeft = 1;
                            while (retriesLeft > 0)
                            {
                                pivSession.TryChangePuk(System.Text.Encoding.UTF8.GetBytes("87654321"), System.Text.Encoding.UTF8.GetBytes("23456789"), out retriesLeft);
                            }
                            if (YubiKeyModule._pivPIN is not null && YubiKeyModule._pivPIN.Length > 0 && Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!)) != "123456")
                            {
                                WriteDebug("Trying to revert PIN");
                                pivSession.TryChangePin(System.Text.Encoding.UTF8.GetBytes("123456"), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(YubiKeyModule._pivPIN!))!), out retriesLeft);
                            }
                            else
                            {
                                WriteWarning("PIN not set, remember to change.");
                            }
                        }
                        else
                        {
                            WriteWarning("PIN and PUK codes reset to default, remember to change.");
                        }
                        break;
                    case "ChangePIN":
                        try
                        {
                            if (pivSession.TryChangePin(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PIN))!)
    , System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPIN!))!)
    , out retriesLeft) == true)
                            {
                                YubiKeyModule._pivPIN = NewPIN;
                            }
                            else
                            {
                                throw new Exception("Incorrect PIN provided");
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
                            if (pivSession.TryChangePuk(System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(PUK))!), System.Text.Encoding.UTF8.GetBytes(Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(NewPUK))!)
    , out retriesLeft) == false)
                            {
                                throw new Exception("Incorrect PUK provided");
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
                        byte[] ManagementKeyarray = (byte[])ManagementKey.BaseObject;
                        byte[] NewManagementKeyarray = (byte[])NewManagementKey.BaseObject;
                        try
                        {
                            if (pivSession.TryChangeManagementKey(ManagementKeyarray, NewManagementKeyarray, (PivTouchPolicy)TouchPolicy, (PivAlgorithm)Algorithm))
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
                            pivSession.WriteObject(chuid);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Failed to generate new CHUID", e);
                        }
                        break;
                    case "Set Managementkey to PIN protected":
                        PivAlgorithm mgmtKeyAlgorithm = ((YubiKeyDevice)YubiKeyModule._yubikey!).HasFeature(YubiKeyFeature.PivAesManagementKey) ? PivAlgorithm.Aes256 : PivAlgorithm.TripleDes;
                        pivSession.SetPinOnlyMode(PivPinOnlyMode.PinProtected, mgmtKeyAlgorithm);
                        break;
                }
            }
        }
    }
}