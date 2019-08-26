using System;

namespace Oakton
{
    /// <summary>
    /// If the CommandExecutor is configured to discover assemblies,
    /// this attribute on an assembly will cause Oakton to search for
    /// command types within this assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class OaktonCommandAssemblyAttribute : Attribute
    {
        
    }
}