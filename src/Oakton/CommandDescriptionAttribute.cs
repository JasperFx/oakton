using System;

namespace Oakton
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandDescriptionAttribute : Attribute
    {
        public CommandDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }

        public string Name { get; set; }
    }
}