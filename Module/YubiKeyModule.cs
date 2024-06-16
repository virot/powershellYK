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
        public static YKKeyCollector _KeyCollector = new YKKeyCollector();
        public static SecureString? _pivPIN;
        public static SecureString? _fido2PIN;
        public static SecureString? _OATHPassword;
        public static byte[] _pivManagementKey = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
    }
    public class MyModuleAssemblyInitializer: IModuleAssemblyInitializer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public void OnImport()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsOnly.AddDllDirectory();
            }
        }
    }

    public class MyModuleAssemblyCleanup: IModuleAssemblyCleanup
    {
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            if (YubiKeyModule._connection is not null)
            {
                YubiKeyModule._connection.Dispose();
            }
        }
    }

    public class WindowsOnly
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public static void AddDllDirectory()
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


}
