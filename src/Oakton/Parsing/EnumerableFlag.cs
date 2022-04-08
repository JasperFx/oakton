using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;

namespace Oakton.Parsing
{
    public interface IFlag
    {
        string ShorthandUsage { get; }
        string LonghandUsage { get; }
        string Description { get; }
    }
    
    public class EnumerableFlag : Flag
    {
        private readonly MemberInfo _member;

        public EnumerableFlag(MemberInfo member, Conversions conversions)
            : base(member, member.GetMemberType().DeriveElementType(), conversions)
        {
            _member = member;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var elementType = _member.GetMemberType().DeriveElementType();
            var list = typeof(List<>).CloseAndBuildAs<IList>(elementType);

            var wasHandled = false;

            if (tokens.NextIsFlagFor(_member))
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
            var flagAliases = InputParser.ToFlagAliases(_member);

            var name = InputParser.RemoveFlagSuffix(_member.Name).ToLower();
            return "[{0} <{1}1 {1}2 {1}3 ...>]".ToFormat(flagAliases, name);
        }
    }
}