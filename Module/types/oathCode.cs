using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace VirotYubikey.OATH.Code
{
    public class TOTP
    {
        public string Issuer { get; }
        public string AccountName { get; }
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
        public string Issuer { get; }
        public string AccountName { get; }
        public string Value { get; }

        public HOTP(string issuer, string accountName, string value)
        {
            Issuer = issuer;
            AccountName = accountName;
            Value = value;
        }
    }
}
