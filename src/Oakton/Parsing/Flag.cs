using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public class Flag : TokenHandlerBase
    {
        private readonly MemberInfo _member;
        protected Func<string, object> Converter;

        public Flag(MemberInfo member, Conversions conversions) : this(member, member.GetMemberType(), conversions)
        {

        }

        public Flag(MemberInfo member, Type propertyType, Conversions conversions) : base(member)
        {
            _member = member;
            Converter = conversions.FindConverter(propertyType);

            if (Converter == null)
            {
                throw new ArgumentOutOfRangeException($"Cannot derive a conversion for type {member.GetMemberType()} on property {member.Name}");
            }
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlagFor(_member))
            {
                var flag = tokens.Dequeue();

                if( tokens.Count == 0 ) throw new InvalidUsageException("No value specified for flag {0}.".ToFormat(flag));

                var rawValue = tokens.Dequeue();
                var value = Converter(rawValue);

                setValue(input, value);

                return true;
            }


            return false;
        }

        public override string ToUsageDescription()
        {
            var flagAliases = InputParser.ToFlagAliases(_member);

            if (_member.GetMemberType().GetTypeInfo().IsEnum)
            {
                var enumValues = Enum.GetNames(_member.GetMemberType()).Join("|");
                return $"[{flagAliases} {enumValues}]";
            }

            var name = _member.Name.ToLower();
            
            if (name.EndsWith("flag") && name.Length > 4)
            {
                name = name.Substring(0, name.Length - 4);
            }

            return $"[{flagAliases} <{name}>]";
        }
    }
}