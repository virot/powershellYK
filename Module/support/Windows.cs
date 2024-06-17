using System.Reflection;
using System.Runtime.InteropServices;

namespace powershellYK.support
{
    public class Windows
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr AddDllDirectory(string NewDirectory);

        public static void AddDllDirectory()
        {
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
    }

}
