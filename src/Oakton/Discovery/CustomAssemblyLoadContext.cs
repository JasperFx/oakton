#if NETSTANDARD2_0
using System.Reflection;
using System.Runtime.Loader;

namespace Oakton.Discovery
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
#endif