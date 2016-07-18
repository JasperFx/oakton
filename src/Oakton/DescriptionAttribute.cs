using System;

namespace Oakton
{
    /// <summary>
    /// Adds a textual description to arguments or flags on an Oakton input class
    /// </summary>
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