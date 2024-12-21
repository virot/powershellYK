using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace powershellYK.UserValidation
{
    public class Fingerprint
    {
        [Hidden]
        public ReadOnlyMemory<Byte> TemplateID { get; }
        public string ID { get { return BitConverter.ToString(this.TemplateID.ToArray()).Replace("-", ""); } }
        public string Name { get; }

        public Fingerprint(ReadOnlyMemory<Byte> templateID, string? friendlyName)
        {
            this.TemplateID = templateID;
            this.Name = friendlyName ?? "";
        }
    }
}
