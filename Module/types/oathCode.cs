using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace powershellYK.OATH.Code
{
    public class TOTP
    {
        [Hidden]
        public string Issuer { get; }
        [Hidden]
        public string AccountName { get; }
        public string Name { get { return $"{Issuer}:{AccountName}"; } }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidUntil { get; }
        public string Value { get; }


        public TOTP(string issuer, string accountName, DateTimeOffset? validFrom, DateTimeOffset? validUntil, string value)
        {
            Issuer = issuer;
            AccountName = accountName;
            ValidFrom = validFrom != null ? validFrom.Value.ToLocalTime().DateTime : null;
            ValidUntil = validUntil != null ? validUntil.Value.ToLocalTime().DateTime : null;
            Value = value;
        }
    }
    public class HOTP
    {
        [Hidden]
        public string Issuer { get; }
        [Hidden]
        public string AccountName { get; }
        public string Name { get { return $"{Issuer}:{AccountName}"; } }
        public string Value { get; }


        public HOTP(string issuer, string accountName, string value)
        {
            Issuer = issuer;
            AccountName = accountName;
            Value = value;
        }
    }
}
