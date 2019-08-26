using System.IO;
using System.Reflection;

namespace Oakton.Discovery
{
    internal interface IOaktonAssemblyLoadContext
    {
        Assembly LoadFromStream(Stream assembly);
        Assembly LoadFromAssemblyName(AssemblyName assemblyName);
        Assembly LoadFromAssemblyPath(string assemblyName);
    }
}