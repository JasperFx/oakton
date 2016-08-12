using System;

namespace Oakton
{
    /// <summary>
    /// Service locator for command types. The default just uses Activator.CreateInstance().
    /// Can be used to plug in IoC construction in Oakton applications
    /// </summary>
    public interface ICommandCreator
    {
        IOaktonCommand Create(Type commandType);
    }
}
