using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public class Flag : TokenHandlerBase
    {
        private readonly PropertyInfo _property;
        protected Func<string, object> Converter;

        public Flag(PropertyInfo property, Conversions conversions) : this(property, property.PropertyType, conversions)
        {

        }

        public Flag(PropertyInfo property, Type propertyType, Conversions conversions) : base(property)
        {
            _property = property;
            Converter = conversions.FindConverter(propertyType);

            if (Converter == null)
            {
                throw new ArgumentOutOfRangeException($"Cannot derive a conversion for type {property.PropertyType} on property {property.Name}");
            }
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
                return $"[{flagAliases} {enumValues}]";
            }

            
            return $"[{flagAliases} <{_property.Name.ToLower().TrimEnd('f', 'l','a','g')}>]";
        }
    }
}