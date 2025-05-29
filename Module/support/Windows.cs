/// <summary>
/// Provides Windows-specific functionality for YubiKey operations.
/// Handles DLL directory management and administrator privilege checks.
/// 
/// .EXAMPLE
/// # Add DLL directory for native libraries
/// [powershellYK.support.Windows]::AddDllDirectory()
/// 
/// .EXAMPLE
/// # Check if running with administrator privileges
/// $isAdmin = [powershellYK.support.Windows]::IsRunningAsAdministrator()
/// if ($isAdmin) {
///     Write-Host "Running with administrator privileges"
/// }
/// </summary>

// Imports
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace powershellYK.support
{
    // Windows-specific functionality for YubiKey operations
    public class Windows
    {
        // Native Windows API import for adding DLL directories
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        // Adds the appropriate native DLL directory based on system architecture
        public static void AddDllDirectory()
        {
            // Get the assembly location and directory
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyPath = Path.GetDirectoryName(assemblyLocation)!;
            if (RuntimeInformation.OSArchitecture == Architecture.X64)
            {
                string runtimePath = assemblyPath != null ? Path.Combine(assemblyPath, "runtimes\\win-x64\\native") : "";
                IntPtr result = AddDllDirectory(runtimePath);
            }
            else if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                string runtimePath = assemblyPath != null ? Path.Combine(assemblyPath, "runtimes\\win-arm64\\native") : "";
                IntPtr result = AddDllDirectory(runtimePath);
                // Note: Error handling is intentionally omitted as per original code
            }
            else if (RuntimeInformation.OSArchitecture == Architecture.Arm)
            {
                string runtimePath = assemblyPath != null ? Path.Combine(assemblyPath, "runtimes\\win-\\native") : "";
                IntPtr result = AddDllDirectory(runtimePath);
            }
            else if (RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                string runtimePath = assemblyPath != null ? Path.Combine(assemblyPath, "runtimes\\win-x86\\native") : "";
                IntPtr result = AddDllDirectory(runtimePath);
            }

            /* We are not really handling the error here.
            if (result == IntPtr.Zero)
            {
                // Call failed, you can get the error code by calling Marshal.GetLastWin32Error()
                int errorCode = Marshal.GetLastWin32Error();
            }
            */
        }
        public static bool IsRunningAsAdministrator()
        {
            // Only check on Windows platform
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            else
            {
                return true;
            }
        }
    }

}
