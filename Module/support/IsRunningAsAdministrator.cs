using System.Security.Principal;

namespace VirotYubikey.support
{
    public class PermisionsStuff
    {
        public static bool IsRunningAsAdministrator()
        {
#if WINDOWS
#pragma warning disable CA1416 // Validate platform compatibility
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
#pragma warning restore CA1416 // Validate platform compatibility
#else 
         return false;
#endif //WINDOWS
        }
    }
}
