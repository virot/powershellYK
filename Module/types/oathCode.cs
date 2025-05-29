/// <summary>
/// Represents OATH TOTP and HOTP codes for YubiKey authentication.
/// Provides time-based and counter-based one-time password functionality.
/// 
/// .EXAMPLE
/// # Create a TOTP code
/// $totp = [powershellYK.OATH.Code.TOTP]::new(
///     "Example",
///     "user@example.com",
///     $validFrom,
///     $validUntil,
///     "123456"
/// )
/// Write-Host "TOTP Code: $($totp.Value)"
/// 
/// .EXAMPLE
/// # Create an HOTP code
/// $hotp = [powershellYK.OATH.Code.HOTP]::new(
///     "Example",
///     "user@example.com",
///     "123456"
/// )
/// Write-Host "HOTP Code: $($hotp.Value)"
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace powershellYK.OATH.Code
{
    // Represents a Time-based One-Time Password (TOTP)
    public class TOTP
    {
        // Issuer of the TOTP (hidden from PowerShell)
        [Hidden]
        public string Issuer { get; }

        // Account name for the TOTP (hidden from PowerShell)
        [Hidden]
        public string AccountName { get; }

        // Display name combining issuer and account
        public string Name { get { return $"{Issuer}:{AccountName}"; } }

        // Start time of validity period
        public DateTime? ValidFrom { get; }

        // End time of validity period
        public DateTime? ValidUntil { get; }

        // Current TOTP value
        public string Value { get; }

        // Creates a new TOTP instance
        public TOTP(string issuer, string accountName, DateTimeOffset? validFrom, DateTimeOffset? validUntil, string value)
        {
            Issuer = issuer;
            AccountName = accountName;
            ValidFrom = validFrom != null ? validFrom.Value.ToLocalTime().DateTime : null;
            ValidUntil = validUntil != null ? validUntil.Value.ToLocalTime().DateTime : null;
            Value = value;
        }
    }

    // Represents a HMAC-based One-Time Password (HOTP)
    public class HOTP
    {
        // Issuer of the HOTP (hidden from PowerShell)
        [Hidden]
        public string Issuer { get; }

        // Account name for the HOTP (hidden from PowerShell)
        [Hidden]
        public string AccountName { get; }

        // Display name combining issuer and account
        public string Name { get { return $"{Issuer}:{AccountName}"; } }

        // Current HOTP value
        public string Value { get; }

        // Creates a new HOTP instance
        public HOTP(string issuer, string accountName, string value)
        {
            Issuer = issuer;
            AccountName = accountName;
            Value = value;
        }
    }
}
