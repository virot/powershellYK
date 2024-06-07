using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace powershellYK.Cmdlets.Yubikey
{
    [Cmdlet(VerbsCommon.Find, "Yubikey")]
    public class FindYubikeyCommand : Cmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only one Yubikey")]
        public SwitchParameter OnlyOne { get; set; }
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only yubikey with serialnumber")]
        public int? Serialnumber { get; set; }

        protected override void BeginProcessing()


        {
            WriteDebug("ProcessRecord in Get-Yubikey");
            var yubiKeys = YubiKeyDevice.FindAll();

            if (Serialnumber is not null)
            {
                //Filter out the yubikeys that does not match the serialnumber
                yubiKeys = yubiKeys.Where(yubiKeys => yubiKeys.SerialNumber == Serialnumber);
            }


            if (yubiKeys.Count() == 1)
            {
                WriteObject(yubiKeys.First());
            }
            else if (yubiKeys.Count() == 0)
            {
                throw new ItemNotFoundException("No Yubikey found");
            }
            else if (OnlyOne.IsPresent)
            {
                throw new Exception("Multiple Yubikeys found");
            }
            else
            {
                WriteObject(yubiKeys.ToArray());
            }


        }
    }
}