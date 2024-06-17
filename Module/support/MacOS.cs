using System.Reflection;
using System.Runtime.InteropServices;

namespace powershellYK.support
{
    class MacOS
    {
        public static IntPtr ResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // First, try to load the library using the default search paths
            IntPtr handle;
            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle))
            {
                return handle;
            }

            if(libraryName == "Yubico.NativeShims")
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    if (NativeLibrary.TryLoad("./runtimes/osx-arm64/native/libYubico.NativeShims.dylib", out handle))
                    {
                        return handle;
                    }
                }
                else
                {
                    if (NativeLibrary.TryLoad("./runtimes/osx-x64/native/libYubico.NativeShims.dylib", out handle))
                    {
                        return handle;
                    }
                }
            }

            // If none of the paths work, throw an exception
            throw new DllNotFoundException($"Unable to load library: {libraryName} from any provided paths or default system paths.");
        }
    }
}
