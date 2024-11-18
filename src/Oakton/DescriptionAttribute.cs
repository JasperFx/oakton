using System;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     Adds a textual description to arguments or flags on input classes, or on a command class
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string description)
    {
        Description = description;
    }

    public string Description { get; set; }

    public string Name { get; set; }
}