using Yubico.YubiKey;
using Yubico.YubiKey.Piv;
using Yubico.YubiKey.Fido2;
using System.Management.Automation;
using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Yubico.YubiKey.Oath;

namespace VirotYubikey
{
    public static class YubiKeyModule
    {
        public static YubiKeyDevice? _yubikey;
        public static IYubiKeyConnection? _connection;
        public static PivSession? _pivSession;
        public static Fido2Session? _fido2Session;
        public static OathSession? _oathSession;
    }
#if WINDOWS
    public class MyModuleAssemblyInitializer: IModuleAssemblyInitializer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public void OnImport()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyPath = Path.GetDirectoryName(assemblyLocation);
#if PUBLISH
            string runtimePath = assemblyPath;
#else //NOT PUBLISH
            string runtimePath = Path.Combine(assemblyPath, "runtimes\\win-x64\\native");
#endif //PUBLISH
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
            if (YubiKeyModule._pivSession is not null)
            {
                YubiKeyModule._pivSession.Dispose();
            }
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
