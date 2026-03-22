// taken from https://github.com/PalmEmanuel/Isol8/blob/main/Source/Assets/ModuleIsolation.cs

using System.Reflection;
using System.Management.Automation;
using System.Runtime.Loader;
using System.IO;
using System.Diagnostics;

namespace powershellYK_loader
{

    // Implement interfaces for interacting with loading logic of PowerShell
    public abstract class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        // Create a new custom ALC and provide the directory
        private static Isol8AssemblyLoadContext? alc;

        // Keep the dependencyDirectory on the instance to avoid static init ordering problems
        private readonly string dependencyDirectory;

        public ModuleInitializer(string assemblyName)
        {
            ModuleName = assemblyName;

            // Resolve directory at runtime (fall back to AppContext.BaseDirectory)
            dependencyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;

            alc = new Isol8AssemblyLoadContext(dependencyDirectory, assemblyName);
        }

        // Runs when Import-Module is run on our module, but in this case also when referred to in NestedModules
        public void OnImport() => AssemblyLoadContext.Default.Resolving += ResolveAssembly;
        // Runs when user runs Remove-Module on our module
        public void OnRemove(PSModuleInfo psModuleInfo) => AssemblyLoadContext.Default.Resolving -= ResolveAssembly;

        // Name of initializer assembly
        public static string ModuleName { get; set; }

        // Resolve assembly by name if it's the Isol8 dll being loaded by the default ALC
        // We know it's the default ALC because of OnImport above
        public static Assembly? ResolveAssembly(AssemblyLoadContext defaultAlc, AssemblyName assemblyName)
        {
            try
            {
                // Defensive: ensure alc exists
                if (alc is null)
                {
                    Trace.WriteLine("ResolveAssembly: custom ALC is null.");
                    return null;
                }

                // Quick path: if the requested assembly name matches our target module name, load by name
                if (string.Equals(assemblyName.Name, ModuleName, StringComparison.OrdinalIgnoreCase))
                {
                    Trace.WriteLine($"ResolveAssembly: attempting LoadFromAssemblyName for {assemblyName.Name}");
                    try
                    {
                        return alc.LoadFromAssemblyName(assemblyName);
                    }
                    catch (FileNotFoundException fnf)
                    {
                        Trace.WriteLine($"ResolveAssembly: LoadFromAssemblyName failed: {fnf.Message}");
                        // fall through to try loading from file path
                    }
                }

                // Try to locate the assembly file in the dependency directory and load it directly
                var probePath = Path.Combine(alc.DependencyDirectory, $"{assemblyName.Name}.dll");
                Trace.WriteLine($"ResolveAssembly: probing path {probePath}");
                if (File.Exists(probePath))
                {
                    try
                    {
                        return alc.LoadFromAssemblyPath(probePath);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"ResolveAssembly: LoadFromAssemblyPath failed for {probePath}: {ex}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"ResolveAssembly: unexpected exception: {ex}");
            }

            // Returning null lets the runtime continue its normal resolution
            return null;
        }
    }

    // We create our own ALC by inheriting from AssemblyLoadContext and overriding the Load() method
    // We can also change the constructor to take a path which we load from, which we do here
    public class Isol8AssemblyLoadContext : AssemblyLoadContext
    {
        // The path which we try to load the assemblies from
        private readonly string dependencyDirectory;

        // Expose dependency directory for probing from ResolveAssembly
        public string DependencyDirectory => dependencyDirectory;

        // We can call the base constructor to set a name for the ALC
        // There are more options such as marking our ALC as collectible to enable unloading it, but that doesn't work with PowerShell
        public Isol8AssemblyLoadContext(string path, string moduleName) : base(moduleName)
        {
            dependencyDirectory = path;
        }

        // Override the Load() method and try to load the module as a DLL file in the provided directory if it exists
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            try
            {
                var assemblyPath = Path.Join(dependencyDirectory, $"{assemblyName.Name}.dll");

                Trace.WriteLine($"Isol8ALC.Load: probing {assemblyPath}");

                // If it exists we can load it from the path
                if (File.Exists(assemblyPath))
                {
                    try
                    {
                        return LoadFromAssemblyPath(assemblyPath);
                    }
                    catch (FileLoadException flex)
                    {
                        Trace.WriteLine($"Isol8ALC.Load: FileLoadException when loading {assemblyPath}: {flex}");
                        return null;
                    }
                    catch (BadImageFormatException bife)
                    {
                        Trace.WriteLine($"Isol8ALC.Load: BadImageFormatException when loading {assemblyPath}: {bife}");
                        return null;
                    }
                }

                // As a last resort try enumerating dlls that start with the assembly name (helps if file naming differs)
                foreach (var file in Directory.EnumerateFiles(dependencyDirectory, $"{assemblyName.Name}*.dll"))
                {
                    Trace.WriteLine($"Isol8ALC.Load: trying enumerated file {file}");
                    try
                    {
                        return LoadFromAssemblyPath(file);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Isol8ALC.Load: failed to load {file}: {ex}");
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Trace.WriteLine($"Isol8ALC.Load: dependency directory not found: {dependencyDirectory}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Isol8ALC.Load: unexpected exception: {ex}");
            }

            // Returning null once more lets the loader know that we didn't load the module, and lets it try something else
            return null;
        }

    }
    public class PowershellYKModuleInitializer : ModuleInitializer
    {
        public PowershellYKModuleInitializer() : base("powershellYK") { }
    }
}
