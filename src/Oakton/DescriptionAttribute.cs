using System;

namespace Oakton
{
    /// <summary>
    /// Adds a textual description to arguments or flags on input classes, or on a command class
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Name { get; set; }
    }
}