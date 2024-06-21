using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Yubico.YubiKey.Piv;
using System.Management.Automation;
using Yubico.YubiKey;

namespace powershellYK
{
    public class powershellYKInfo
    {
        public Version? YubicoVersion { get; }
        public Version? AutomationVersion { get; }
        public Version? powershellYKVersion { get; }

        public powershellYKInfo(
            Version? YubicoVersion,
            Version? AutomationVersion,
            Version? powershellYKVersion)
        {
            this.YubicoVersion = YubicoVersion;
            this.AutomationVersion = AutomationVersion;
            this.powershellYKVersion = powershellYKVersion;
        }
    }
}
