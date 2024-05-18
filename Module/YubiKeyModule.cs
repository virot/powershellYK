using Yubico.YubiKey;
using Yubico.YubiKey.Piv;

namespace Yubikey_Powershell
{
    public static class YubiKeyModule
    {
        public static YubiKeyDevice? _yubikey;
        public static IYubiKeyConnection? _connection;
        public static PivSession? _pivSession;
    }
}
