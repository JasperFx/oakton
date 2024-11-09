using System;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     Service locator for command types. The default just uses Activator.CreateInstance().
///     Can be used to plug in IoC construction in Oakton applications
/// </summary>
public interface ICommandCreator
{
    IOaktonCommand CreateCommand(Type commandType);
    object CreateModel(Type modelType);
}