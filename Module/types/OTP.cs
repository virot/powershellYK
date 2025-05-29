/// <summary>
/// Represents OTP (One-Time Password) information and configurations for YubiKey.
/// Provides classes for managing OTP slots, Yubico OTP, and challenge-response operations.
/// 
/// .EXAMPLE
/// # Get OTP slot information
/// $info = [powershellYK.OTP.Info]::new("1", $true, $false)
/// Write-Host "Slot $($info.Slot) is configured: $($info.Configured)"
/// 
/// .EXAMPLE
/// # Create Yubico OTP configuration
/// $yubicoOTP = [powershellYK.OTP.YubicoOTP]::new(
///     $serial,
///     $publicId,
///     $privateId,
///     $secretKey,
///     $onboardUrl
/// )
/// Write-Host "Public ID: $($yubicoOTP.PublicID)"
/// </summary>

// Imports
using powershellYK.support;

namespace powershellYK.OTP
{
    // Represents information about an OTP slot
    public class Info
    {
        // Slot identifier
        public string Slot { get; }

        // Whether the slot is configured
        public bool Configured { get; }

        // Whether the slot requires touch
        public bool RequiresTouch { get; }

        // Creates a new OTP slot information instance
        public Info(string Slot, bool Configured, bool RequiresTouch)
        {
            this.Slot = Slot;
            this.Configured = Configured;
            this.RequiresTouch = RequiresTouch;
        }
    }

    // Represents a Yubico OTP configuration
    public class YubicoOTP
    {
        // Device serial number
        public int? Serial { get; }

        // Public ID in byte array format
        public byte[] PublicIDByte { get; } = new byte[6];

        // Public ID in string format
        public string PublicID { get; } = "";

        // Private ID in byte array format
        public byte[] PrivateIDByte { get; } = new byte[6];

        // Private ID in string format
        public string PrivateID { get; } = "";

        // Secret key in byte array format
        public byte[] SecretKeyByte { get; } = new byte[16];

        // Secret key in string format
        public String SecretKey { get; } = "";

        // Onboarding URL for the OTP
        public string onboardUrl { get; } = "";

        // Creates a new Yubico OTP configuration instance
        public YubicoOTP(int? serial, byte[] PublicID, byte[] PrivateID, byte[] SecretKey, string onboardUrl)
        {
            this.Serial = serial;
            this.PublicIDByte = PublicID;
            this.PublicID = Converter.ByteArrayToString(this.PublicIDByte);
            this.PrivateIDByte = PrivateID;
            this.PrivateID = Converter.ByteArrayToString(this.PrivateIDByte);
            this.SecretKeyByte = SecretKey;
            this.SecretKey = Converter.ByteArrayToString(this.SecretKeyByte);
            this.onboardUrl = onboardUrl;
        }
    }

    // Represents a challenge-response configuration
    public class ChallangeResponse
    {
        // Secret key in byte array format
        public byte[] SecretKeyByte { get; }

        // Secret key in string format
        public String SecretKey { get; } = "";

        // Creates a new challenge-response configuration instance
        public ChallangeResponse(byte[] SecretKey)
        {
            this.SecretKeyByte = SecretKey;
            this.SecretKey = Converter.ByteArrayToString(this.SecretKeyByte);
        }
    }
}
