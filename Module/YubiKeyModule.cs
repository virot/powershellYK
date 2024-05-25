using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Fido2;
using System.Management.Automation;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace VirotYubikey
{
    public static class YubiKeyModule
    {
        public static YubiKeyDevice? _yubikey;
        public static IYubiKeyConnection? _connection;
        public static PivSession? _pivSession;
        public static Fido2Session? _fido2Session;
    }
    public class MyModuleAssemblyInitializer: IModuleAssemblyInitializer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public void OnImport()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyPath = Path.GetDirectoryName(assemblyLocation);
            string runtimePath = Path.Combine(assemblyPath, "runtimes\\win-x64\\native");
            IntPtr result = AddDllDirectory(runtimePath);
            if (result == IntPtr.Zero)
            {
                // Call failed, you can get the error code by calling Marshal.GetLastWin32Error()
                int errorCode = Marshal.GetLastWin32Error();
            }
        }
    }
}
