using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Piv.Commands;
using VirotYubikey.support;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;


namespace VirotYubikey.Cmdlets.PIV
{
    [Cmdlet(VerbsCommon.New, "YubikeyPIVKey", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class NewYubiKeyPIVKeyCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "What slot to create a new key for")]

        public byte Slot { get; set; }

        [ValidateSet("Rsa1024", "Rsa2048", "EccP256", "EccP384", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Algoritm")]
        public PivAlgorithm Algorithm { get; set; }

        [ValidateSet("Default", "Never", "None", "Once", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "PinPolicy")]
        public PivPinPolicy PinPolicy { get; set; } = PivPinPolicy.Default;

        [ValidateSet("Default", "Never", "Always", "Cached", IgnoreCase = true)]
        [Parameter(Position = 0, Mandatory = false, ValueFromPipeline = false, HelpMessage = "TouchPolicy")]
        public PivTouchPolicy TouchPolicy { get; set; } = PivTouchPolicy.Default;

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

            bool keyExists = false;
            try
            {
                PivPublicKey pubkey = YubiKeyModule._pivSession.GetMetadata(Slot).PublicKey;
                keyExists = true;
            }
            catch { }

            if ( ! keyExists || ShouldProcess($"Slot 0x{Slot.ToString("X2")}", "New"))
            {
                try
                {
                    WriteDebug("ProcessRecord in New-YubikeyPIVKey");
                    PivPublicKey publicKey = YubiKeyModule._pivSession!.GenerateKeyPair(Slot, Algorithm, PinPolicy, TouchPolicy);
                    if (publicKey is not null) { 
                        WriteObject("KeyPair created");
                    }
                    else { throw new Exception("Could not create keypair"); }
                }
                catch (Exception e)
                {
                    throw new Exception("Could not create keypair", e);
                }

            }
        }
    }
}