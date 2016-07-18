using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton
{
    public class Flag : TokenHandlerBase
    {
        private readonly PropertyInfo _property;
        protected Func<string, object> Converter;

        public Flag(PropertyInfo property, Conversions conversions) : base(property)
        {
            _property = property;
            Converter = conversions.FindConverter(property.PropertyType);
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlagFor(_property))
            {
                var flag = tokens.Dequeue();

                if( tokens.Count == 0 ) throw new InvalidUsageException("No value specified for flag {0}.".ToFormat(flag));

                var rawValue = tokens.Dequeue();
                var value = Converter(rawValue);

                _property.SetValue(input, value, null);

                return true;
            }


            return false;
        }

        public override string ToUsageDescription()
        {
            var flagAliases = InputParser.ToFlagAliases(_property);

            if (_property.PropertyType.GetTypeInfo().IsEnum)
            {
                var enumValues = Enum.GetNames(_property.PropertyType).Join("|");
                return "[{0} {1}]".ToFormat(flagAliases, enumValues);
            }

            
            return "[{0} <{1}>]".ToFormat(flagAliases, _property.Name.ToLower().TrimEnd('f', 'l','a','g'));
        }
    }
}