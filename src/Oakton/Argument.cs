using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton
{
    public class Argument : TokenHandlerBase
    {
        private readonly PropertyInfo _property;
        private bool _isLatched;
        protected Func<string, object> _converter;

        public Argument(PropertyInfo property, Conversions conversions) : base(property)
        {
            _property = property;
            _converter = conversions.FindConverter(property.PropertyType);
        }

        public ArgumentReport ToReport()
        {
            return new ArgumentReport
            {
                Description = Description,
                Name = _property.Name.ToLower()
            };
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (_isLatched) return false;

            if (tokens.NextIsFlag()) return false;

            var value = _converter(tokens.Dequeue());
            _property.SetValue(input, value, null);

            _isLatched = true;

            return true;
        }

        public override string ToUsageDescription()
        {
            if (_property.PropertyType.GetTypeInfo().IsEnum)
            {
                return Enum.GetNames(_property.PropertyType).Join("|");
            }

            return $"<{_property.Name.ToLower()}>";
        }
    }
}