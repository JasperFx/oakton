using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public class EnumerableArgument : Argument
    {
        private readonly PropertyInfo _property;

        public EnumerableArgument(PropertyInfo property, Conversions conversions) : base(property, conversions)
        {
            _property = property;

            _converter = conversions.FindConverter(property.PropertyType.DeriveElementType());
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var elementType = _property.PropertyType.GetGenericArguments().First();
            var list = typeof (List<>).CloseAndBuildAs<IList>(elementType);

            var wasHandled = false;
            while (tokens.Count > 0 && !tokens.NextIsFlag())
            {
                var value = _converter(tokens.Dequeue());
                list.Add(value);

                wasHandled = true;
            }

            if (wasHandled)
            {
                _property.SetValue(input, list, null);
            }

            return wasHandled;
        }

        public override string ToUsageDescription()
        {
            return "[<{0}1 {0}2 {0}3 ...>]".ToFormat(_property.Name.ToLower());
        }
    }
}