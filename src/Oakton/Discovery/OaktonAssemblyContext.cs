namespace Oakton.Discovery
{
#if NETSTANDARD2_0
    internal static class OaktonAssemblyContext
    {
        public static readonly IOaktonAssemblyLoadContext Loader = new AssemblyLoadContextWrapper(System.Runtime.Loader.AssemblyLoadContext.Default);
    }
#endif
}