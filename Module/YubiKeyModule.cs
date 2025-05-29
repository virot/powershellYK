/// <summary>
/// Core module for PowerShell YubiKey integration.
/// Provides YubiKey device management, connection handling, and PIN management.
/// 
/// .EXAMPLE
/// # Connect to YubiKey
/// ConnectYubikey()
/// Write-Host "Connected to YubiKey"
/// 
/// .EXAMPLE
/// # Set PIV PIN
/// $pin = ConvertTo-SecureString "123456" -AsPlainText -Force
/// setPIVPIN($pin)
/// </summary>

// Imports
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
using Yubico.Core.Logging;
using Microsoft.Extensions.Logging;

namespace powershellYK
{
    // Core module for PowerShell YubiKey integration
    public static class YubiKeyModule
    {
        // Current YubiKey device instance
        public static YubiKeyDevice? _yubikey;

        // Current YubiKey connection
        public static IYubiKeyConnection? _connection;

        // Key collector for handling PIN and password prompts
        public static YKKeyCollector _KeyCollector = new YKKeyCollector();

        // PIV PIN storage
        public static SecureString? _pivPIN;

        // FIDO2 PIN storage
        public static SecureString? _fido2PIN;

        // New FIDO2 PIN storage (for PIN changes)
        public static SecureString? _fido2PINNew;

        // OATH password storage
        public static SecureString? _OATHPassword;

        // New OATH password storage (for password changes)
        public static SecureString? _OATHPasswordNew;
        public static byte[] _pivManagementKey = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

        // Sets the PIV PIN and handles biometric device special cases
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

        // Sets the FIDO2 PIN and handles biometric device special cases
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

        // Connects to a YubiKey device
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

        // Clears all stored passwords and PINs
        public static void clearPassword()
        {
            _pivPIN = null;
            _fido2PIN = null;
            _fido2PINNew = null;
            _OATHPassword = null;
            _OATHPasswordNew = null;
        }
    }

    // Module assembly initializer for platform-specific setup
    public class MyModuleAssemblyInitializer : IModuleAssemblyInitializer
    {
        // Windows API for adding DLL directories
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        // Initializes the module assembly
        public void OnImport()
        {
            // Platform-specific initialization
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

            // Disable the Yubico SDK logging
            // This can be reenabled by **Enable-PowershellYKLogging**
            Log.ConfigureLoggerFactory(builder => builder.ClearProviders());
        }
    }

    // Module assembly cleanup for resource disposal
    public class MyModuleAssemblyCleanup : IModuleAssemblyCleanup
    {
        // Cleans up resources when the module is removed
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            if (YubiKeyModule._connection is not null)
            {
                YubiKeyModule._connection.Dispose();
            }
        }
    }
}
