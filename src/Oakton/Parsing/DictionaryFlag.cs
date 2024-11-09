using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JasperFx.Core.Reflection;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public class DictionaryFlag : TokenHandlerBase
{
    private readonly string _prefix;

    public DictionaryFlag(MemberInfo member) : base(member)
    {
        if (!member.GetMemberType().CanBeCastTo<IDictionary<string, string>>())
        {
            throw new ArgumentOutOfRangeException("Dictionary flag types have to be IDictionary<string, string>");
        }

        var flagAliases = InputParser.ToFlagAliases(Member);

        _prefix = flagAliases.LongForm + ":";
    }


    public override bool Handle(object input, Queue<string> tokens)
    {
        if (tokens.Peek().StartsWith(_prefix))
        {
            var flag = tokens.Dequeue();

            if (tokens.Count == 0)
            {
                throw new InvalidUsageException($"No value specified for flag {flag}.");
            }

            var key = flag.Split(':').Last().Trim();
            var rawValue = tokens.Dequeue();

            var dict = getValue(input) as IDictionary<string, string>;
            if (dict == null)
            {
                dict = new Dictionary<string, string>();
                setValue(input, dict);
            }

            if (dict.ContainsKey(key))
            {
                dict[key] = rawValue;
            }
            else
            {
                dict.Add(key, rawValue);
            }

            return true;
        }

        return false;
    }

    public override string ToUsageDescription()
    {
        return $"[{_prefix}<prop> <value>]";
    }
}