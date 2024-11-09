using System;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
/// Decorate Oakton commands that are being called by 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InjectServiceAttribute : Attribute
{
    
}