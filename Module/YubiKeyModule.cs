using Yubico.YubiKey;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.support;
using System;
using System.Reflection;
using System.Reflection.Metadata;

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
        public static SecureString? _OATHPasswordNew;
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
                Windows.AddDllDirectory();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                nint handle;
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    NativeLibrary.TryLoad(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/runtimes/osx-arm64/native/libYubico.NativeShims.dylib", out handle);
                }
                else
                {
                    NativeLibrary.TryLoad(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/runtimes/osx-x64/native/libYubico.NativeShims.dylib", out handle);
                }
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

   

}
