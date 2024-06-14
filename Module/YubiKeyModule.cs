using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Fido2;
using System.Management.Automation;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Yubico.YubiKey.Oath;
using System.Security.Cryptography;
using System.Security;
using System.Net.NetworkInformation;

namespace powershellYK
{
    public static class YubiKeyModule
    {
        public static YubiKeyDevice? _yubikey;
        public static IYubiKeyConnection? _connection;
        public static Fido2Session? _fido2Session;
        public static OathSession? _oathSession;
        public static YKKeyCollector _KeyCollector = new YKKeyCollector();
        public static SecureString? _pivPIN;
        public static byte[] _pivManagementKey = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        public static SecureString? _fido2PIN;
    }
#if WINDOWS
    public class MyModuleAssemblyInitializer: IModuleAssemblyInitializer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public void OnImport()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyPath = Path.GetDirectoryName(assemblyLocation)!;
            string runtimePath = assemblyPath != null ? Path.Combine(assemblyPath, "runtimes\\win-x64\\native") : "";
            IntPtr result = AddDllDirectory(runtimePath);
            if (result == IntPtr.Zero)
            {
                // Call failed, you can get the error code by calling Marshal.GetLastWin32Error()
                int errorCode = Marshal.GetLastWin32Error();
            }
        }
    }
#endif //WINDOWS

    public class MyModuleAssemblyCleanup: IModuleAssemblyCleanup
    {
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            if (YubiKeyModule._fido2Session is not null)
            {
                YubiKeyModule._fido2Session.Dispose();
            }
            if (YubiKeyModule._connection is not null)
            {
                YubiKeyModule._connection.Dispose();
            }
            if (YubiKeyModule._oathSession is not null)
            {
                YubiKeyModule._oathSession.Dispose();
            }
        }
    }

}
