using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Find, "Yubikey")]
    public class FindYubikeyCommand : Cmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only one YubiKey")]
        public SwitchParameter OnlyOne { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only YubiKey with Serial Number")]
        public int? Serialnumber { get; set; }

        protected override void BeginProcessing()


        {
            WriteDebug("ProcessRecord in Get-Yubikey");
            IEnumerable<IYubiKeyDevice> yubiKeys = YubiKeyDevice.FindAll();

            if (Serialnumber is not null)
            {
                //Filter out the yubikeys that does not match the serialnumber
                yubiKeys = yubiKeys.Where(yubiKeys => yubiKeys.SerialNumber == Serialnumber);
            }

            foreach (var yubiKey in yubiKeys)
            {
                WriteObject((YubiKeyDevice)yubiKey);
            }

            if (yubiKeys.ToArray().Length == 0)
            {
                WriteWarning("No YubiKeys found, FIDO-only YubiKeys on Windows requires running as Administrator.");
                if (Serialnumber is not null)
                {
                    throw new Exception("No YubiKey found with the specified serial number.");
                }
            }
        }
    }
}