using System.Management.Automation;           // Windows PowerShell namespace.
using Yubico.YubiKey;

namespace Yubikey_Powershell
{
    [Cmdlet(VerbsCommon.Find, "Yubikey")]
    public class FindYubikeyCommand : Cmdlet
    {
        //add a switch variable call only one, that will only return one yubikey if set to true        //add a switch variable call only one, that will only return one yubikey if set to true
        // set default to false
        [Parameter(Mandatory = false, ValueFromPipeline = false, HelpMessage = "Return only one Yubikey")]
        public SwitchParameter OnlyOne { get; set; }

        protected override void BeginProcessing()


        {
            WriteDebug("ProcessRecord in Get-Yubikey");
            var yubiKeys = YubiKeyDevice.FindAll();
            //return the yubikey or an an array of yubikeys, if no youbikey is found throw [eu.virot.yubikey.nonfound            //return the yubikey or an an array of yubikeys, if no youbikey is found throw [eu.virot.yubikey.nonfound]

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