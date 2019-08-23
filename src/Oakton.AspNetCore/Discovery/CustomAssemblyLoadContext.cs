using System.Reflection;
using System.Runtime.Loader;

namespace Oakton.AspNetCore
{
    public sealed class CustomAssemblyLoadContext : AssemblyLoadContext, IOaktonAssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        Assembly IOaktonAssemblyLoadContext.LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return Load(assemblyName);
        }
    }
}