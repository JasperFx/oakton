using System.Collections.Generic;
using System.Reflection;

namespace Oakton
{
    public class BooleanFlag : TokenHandlerBase
    {
        private readonly PropertyInfo _property;

        public BooleanFlag(PropertyInfo property) : base(property)
        {
            _property = property;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (!tokens.NextIsFlagFor(_property)) return false;
            tokens.Dequeue();
            _property.SetValue(input, true, null);

            return true;
        }

        public override string ToUsageDescription()
        {
            return $"[{InputParser.ToFlagAliases(_property)}]";
        }
    }
}