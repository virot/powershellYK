using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System;
using Yubico.Core.Buffers;

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

    public class YubicoOTP
    {
        public int? Serial { get; }
        public byte[] PublicIDByte { get; } = new byte[6];
        public string PublicID { get; } = "";
        public byte[] PrivateIDByte { get; } = new byte[6];
        public string PrivateID { get; } = "";
        public byte[] SecretKeyByte { get; } = new byte[16];
        public String SecretKey { get; } = "";
        public string onboardUrl { get; } = "";

        public YubicoOTP(int? serial, byte[] PublicID, byte[] PrivateID, byte[] SecretKey, string onboardUrl)
        {
            this.Serial = serial;
            this.PublicIDByte = PublicID;
            this.PublicID = ModHex.EncodeBytes(this.PublicIDByte);
            this.PrivateIDByte = PrivateID;
            this.PrivateID = ModHex.EncodeBytes(this.PrivateIDByte);
            this.SecretKeyByte = SecretKey;
            this.SecretKey= ModHex.EncodeBytes(this.SecretKeyByte);
            this.onboardUrl = onboardUrl;
        }
    }


}
