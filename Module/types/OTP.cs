using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace powershellYK.OTP
{
    public class Info
    {
        public string Slot { get; }
        public bool Configured { get; }
        public bool RequiresTouch { get; }

        public Info(string Slot, bool Configured, bool RequiresTouch)
        {
            this.Slot = Slot;
            this.Configured = Configured;
            this.RequiresTouch = RequiresTouch;
        }
    }
}
