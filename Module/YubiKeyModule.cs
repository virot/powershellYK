using Yubico.YubiKey;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using powershellYK.support;
using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Yubico.YubiKey.Fido2;

namespace powershellYK
{
    public static class YubiKeyModule
    {
        public static YubiKeyDevice? _yubikey;
        public static IYubiKeyConnection? _connection;
        public static YKKeyCollector _KeyCollector = new YKKeyCollector();
        public static SecureString? _pivPIN;
        public static SecureString? _fido2PIN;
        public static SecureString? _fido2PINNew;
        public static SecureString? _OATHPassword;
        public static SecureString? _OATHPasswordNew;
        public static byte[] _pivManagementKey = new byte[] {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

        public static void setPIVPIN(SecureString PIN)
        {
            // In BIO mode, the PIV PIN is the same as the FIDO2 PIN
            if (new List<FormFactor> { FormFactor.UsbABiometricKeychain, FormFactor.UsbCBiometricKeychain }.Contains(((YubiKeyDevice)YubiKeyModule._yubikey!).FormFactor))
            {
                _pivPIN = PIN;
                _fido2PIN = PIN;
            }
            else
            {
                _pivPIN = PIN;
            }
        }
        public static void setFIDO2PIN(SecureString PIN)
        {
            // In BIO mode, the PIV PIN is the same as the FIDO2 PIN
            if (new List<FormFactor> { FormFactor.UsbABiometricKeychain, FormFactor.UsbCBiometricKeychain }.Contains(((YubiKeyDevice)YubiKeyModule._yubikey!).FormFactor))
            {
                _pivPIN = PIN;
                _fido2PIN = PIN;
            }
            else
            {
                _fido2PIN = PIN;
            }
        }

        public static bool ConnectYubikey()
        {
            if (YubiKeyModule._yubikey is null)
            {
                try
                {
                    var myPowersShellInstance = PowerShell.Create(RunspaceMode.CurrentRunspace).AddCommand("Connect-Yubikey");
                    myPowersShellInstance.Invoke();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
            return true;
        }

        public static void clearPassword()
        {
            _pivPIN = null;
            _fido2PIN = null;
            _fido2PINNew = null;
            _OATHPassword = null;
            _OATHPasswordNew = null;
        }
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
