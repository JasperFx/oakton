using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public class EnumerableFlag : Flag
    {
        private readonly PropertyInfo _property;

        public EnumerableFlag(PropertyInfo property, Conversions conversions)
            : base(property, property.PropertyType.DeriveElementType(), conversions)
        {
            _property = property;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var elementType = _property.PropertyType.DeriveElementType();
            var list = typeof(List<>).CloseAndBuildAs<IList>(elementType);

            var wasHandled = false;

            if (tokens.NextIsFlagFor(_property))
            {
                var flag = tokens.Dequeue();
                while (tokens.Count > 0 && !tokens.NextIsFlag())
                {
                    var value = Converter(tokens.Dequeue());
                    list.Add(value);

                    wasHandled = true;
                }

                if(!wasHandled)
                {
                    throw new InvalidUsageException("No values specified for flag {0}.".ToFormat(flag));
                }

                setValue(input, list);
            }

            return wasHandled;
        }

        public override string ToUsageDescription()
        {
            var flagAliases = InputParser.ToFlagAliases(_property);

            return "[{0} [<{1}1 {1}2 {1}3 ...>]]".ToFormat(flagAliases, _property.Name.ToLower().TrimEnd('f', 'l', 'a', 'g'));
            
        }
    }
}