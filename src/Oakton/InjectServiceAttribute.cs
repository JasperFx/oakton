using System;

namespace Oakton;

/// <summary>
/// Decorate Oakton commands that are being called by 
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InjectServiceAttribute : Attribute
{
    
}