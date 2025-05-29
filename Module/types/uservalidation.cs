/// <summary>
/// Represents user validation information for YubiKey operations.
/// Provides fingerprint template management and identification.
/// 
/// .EXAMPLE
/// # Create a fingerprint template
/// $fingerprint = [powershellYK.UserValidation.Fingerprint]::new(
///     $templateID,
///     "User's Fingerprint"
/// )
/// Write-Host "Fingerprint ID: $($fingerprint.ID)"
/// 
/// .EXAMPLE
/// # Get fingerprint information
/// Write-Host "Name: $($fingerprint.Name)"
/// Write-Host "Template ID: $($fingerprint.ID)"
/// </summary>

// Imports
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;

namespace powershellYK.UserValidation
{
    // Represents a fingerprint template for user validation
    public class Fingerprint
    {
        // Template identifier (hidden from PowerShell)
        [Hidden]
        public ReadOnlyMemory<Byte> TemplateID { get; }

        // String representation of the template ID
        public string ID { get { return BitConverter.ToString(this.TemplateID.ToArray()).Replace("-", ""); } }

        // Friendly name for the fingerprint
        public string Name { get; }

        // Creates a new fingerprint template instance
        public Fingerprint(ReadOnlyMemory<Byte> templateID, string? friendlyName)
        {
            this.TemplateID = templateID;
            this.Name = friendlyName ?? "";
        }
    }
}
