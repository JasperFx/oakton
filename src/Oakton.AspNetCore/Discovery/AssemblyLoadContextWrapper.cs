using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Oakton.AspNetCore
{
    public sealed class AssemblyLoadContextWrapper : IOaktonAssemblyLoadContext
    {
        private readonly AssemblyLoadContext ctx;

        public AssemblyLoadContextWrapper(AssemblyLoadContext ctx)
        {
            this.ctx = ctx;
        }

        public Assembly LoadFromStream(Stream assembly)
        {
            return ctx.LoadFromStream(assembly);
        }

        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            return ctx.LoadFromAssemblyName(assemblyName);
        }

        public Assembly LoadFromAssemblyPath(string assemblyName)
        {
            return ctx.LoadFromAssemblyPath(assemblyName);
        }
    }
}