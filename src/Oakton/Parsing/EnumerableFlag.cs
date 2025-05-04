using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JasperFx.Core.Reflection;
using Oakton.Internal.Conversion;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public class EnumerableFlag : Flag
{
    private readonly MemberInfo _member;

    public EnumerableFlag(MemberInfo member, Conversions conversions)
        : base(member, member.GetMemberType().DetermineElementType(), conversions)
    {
        _member = member;
    }

    public override bool Handle(object input, Queue<string> tokens)
    {
        var elementType = _member.GetMemberType().DetermineElementType();
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

            if (!wasHandled)
            {
                throw new InvalidUsageException($"No values specified for flag {flag}.");
            }

            setValue(input, list);
        }

        return wasHandled;
    }

    public override string ToUsageDescription()
    {
        var flagAliases = InputParser.ToFlagAliases(_member);

        var name = InputParser.RemoveFlagSuffix(_member.Name).ToLower();
        return $"[{flagAliases} <{name}1 {name}2 {name}3 ...>]";
    }
}