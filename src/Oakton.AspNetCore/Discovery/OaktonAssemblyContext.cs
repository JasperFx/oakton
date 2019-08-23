namespace Oakton.AspNetCore
{
    internal static class OaktonAssemblyContext
    {
        public static readonly IOaktonAssemblyLoadContext Loader = new AssemblyLoadContextWrapper(System.Runtime.Loader.AssemblyLoadContext.Default);
    }
}