// taken from https://github.com/PalmEmanuel/Isol8/blob/main/Source/Assets/ModuleIsolation.cs

using System.Reflection;
using System.Management.Automation;
using System.Runtime.Loader;
using System.IO;

namespace powershellYK_loader
{

    // Implement interfaces for interacting with loading logic of PowerShell
    public abstract class ModuleInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        // Create a new custom ALC and provide the directory
        private static Isol8AssemblyLoadContext alc;
        public ModuleInitializer(string assemblyName)
        {
            ModuleName = assemblyName;
            alc = new Isol8AssemblyLoadContext(dependencyDirectory, assemblyName);
        }

        // Runs when Import-Module is run on our module, but in this case also when referred to in NestedModules
        public void OnImport() => AssemblyLoadContext.Default.Resolving += ResolveAssembly;
        // Runs when user runs Remove-Module on our module
        public void OnRemove(PSModuleInfo psModuleInfo) => AssemblyLoadContext.Default.Resolving -= ResolveAssembly;

        // Name of initializer assembly
        public static string ModuleName { get; set; }
        // Get directory of this assembly, and use that directory to load dependencies from
        private static readonly string dependencyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Resolve assembly by name if it's the Isol8 dll being loaded by the default ALC
        // We know it's the default ALC because of OnImport above
        public static Assembly? ResolveAssembly(AssemblyLoadContext defaultAlc, AssemblyName assemblyName)
        {
            return assemblyName.Name == ModuleName ?
                alc.LoadFromAssemblyName(assemblyName) :
                null;
        }
    }

    // We create our own ALC by inheriting from AssemblyLoadContext and overriding the Load() method
    // We can also change the constructor to take a path which we load from, which we do here
    public class Isol8AssemblyLoadContext : AssemblyLoadContext
    {
        // The path which we try to load the assemblies from
        private readonly string dependencyDirectory;

        // We can call the base constructor to set a name for the ALC
        // There are more options such as marking our ALC as collectible to enable unloading it, but that doesn't work with PowerShell
        public Isol8AssemblyLoadContext(string path, string moduleName) : base(moduleName)
        {
            dependencyDirectory = path;
        }

        // Override the Load() method and try to load the module as a DLL file in the provided directory if it exists
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = Path.Join(dependencyDirectory, $"{assemblyName.Name}.dll");

            // If it exists we can load it from the path
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            // Returning null once more lets the loader know that we didn't load the module, and lets it try something else
            return null;
        }

    }
    public class powershellYKModuleInitializer : ModuleInitializer
    {
        public powershellYKModuleInitializer() : base("powershellYK") { }
    }
}
