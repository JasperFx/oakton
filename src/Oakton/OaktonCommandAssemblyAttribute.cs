using System;
using JasperFx.Core.Reflection;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     If the CommandExecutor is configured to discover assemblies,
///     this attribute on an assembly will cause Oakton to search for
///     command types within this assembly
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class OaktonCommandAssemblyAttribute : Attribute
{
    public OaktonCommandAssemblyAttribute()
    {
    }

    /// <summary>
    ///     Concrete type implementing the IServiceRegistrations interface that should
    ///     automatically be applied to hosts during environment checks or resource
    ///     commands
    /// </summary>
    /// <param name="extensionType"></param>
    public OaktonCommandAssemblyAttribute(Type extensionType)
    {
        if (extensionType.HasDefaultConstructor() && extensionType.CanBeCastTo<IServiceRegistrations>())
        {
            ExtensionType = extensionType;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(extensionType),
                $"Extension types must have a default, no arg constructor and implement the {nameof(IServiceRegistrations)} interface");
        }
    }

    public Type ExtensionType { get; set; }
}