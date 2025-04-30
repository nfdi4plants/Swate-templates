using System.Reflection;

namespace STRService.Components
{
    public class Util
    {
        public static class AppInfo
        {
            public static string Version =>
                (Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                ?? "Unknown").Split('+')[0]; // remove +git hash;
        }
    }
}
