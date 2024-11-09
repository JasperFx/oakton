using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Oakton.Internal.Conversion;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public static class InputParser
{
    private static readonly string LONG_FLAG_PREFIX = "--";
    private static readonly Regex LONG_FLAG_REGEX = new($"^{LONG_FLAG_PREFIX}[^-]+");

    private static readonly string SHORT_FLAG_PREFIX = "-";
    private static readonly Regex SHORT_FLAG_REGEX = new($"^{SHORT_FLAG_PREFIX}[^-]+");

    private static readonly string FLAG_SUFFIX = "Flag";
    private static readonly Conversions _converter = new();


    public static List<ITokenHandler> GetHandlers(Type inputType)
    {
        var properties = inputType.GetProperties().Where(prop => prop.CanWrite);
        var fields = inputType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        var members = properties.OfType<MemberInfo>().Concat(fields);

        return members
            .Where(member => !member.HasAttribute<IgnoreOnCommandLineAttribute>())
            .Select(BuildHandler).ToList();
    }

    public static ITokenHandler BuildHandler(MemberInfo member)
    {
        var memberType = member.GetMemberType();

        if (!member.Name.EndsWith(FLAG_SUFFIX))
        {
            if (memberType.CanBeCastTo<IDictionary<string, string>>())
            {
                throw new ArgumentOutOfRangeException("Dictionaries are only supported as 'Flag' members");
            }

            if (memberType != typeof(string) && memberType.Closes(typeof(IEnumerable<>)))
            {
                return new EnumerableArgument(member, _converter);
            }

            return new Argument(member, _converter);
        }

        // Gotta check this before enumerable!
        if (memberType.CanBeCastTo<IDictionary<string, string>>())
        {
            return new DictionaryFlag(member);
        }

        if (memberType != typeof(string) && memberType.Closes(typeof(IEnumerable<>)))
        {
            return new EnumerableFlag(member, _converter);
        }

        if (memberType == typeof(bool))
        {
            return new BooleanFlag(member);
        }


        return new Flag(member, _converter);
    }

    public static bool IsFlag(string token)
    {
        return IsShortFlag(token) || IsLongFlag(token);
    }

    public static bool IsShortFlag(string token)
    {
        return SHORT_FLAG_REGEX.IsMatch(token);
    }

    public static bool IsLongFlag(string token)
    {
        return LONG_FLAG_REGEX.IsMatch(token);
    }

    public static bool IsFlagFor(string token, MemberInfo property)
    {
        return ToFlagAliases(property).Matches(token);
    }

    public static FlagAliases ToFlagAliases(MemberInfo member)
    {
        var name = RemoveFlagSuffix(member.Name);
        name = splitOnPascalCaseAndAddHyphens(name);

        var oneLetterName = name.ToLower()[0];
        var longFormOnly = false;

        member.ForAttribute<FlagAliasAttribute>(att =>
        {
            name = att.LongAlias ?? name;
            oneLetterName = att.OneLetterAlias ?? oneLetterName;
            longFormOnly = att.LongAliasOnly;
        });

        return new FlagAliases
        {
            ShortForm = SHORT_FLAG_PREFIX + oneLetterName,
            LongForm = LONG_FLAG_PREFIX + name.ToLower(),
            LongFormOnly = longFormOnly
        };
    }

    public static string RemoveFlagSuffix(string fullFlagName)
    {
        var suffixLength = FLAG_SUFFIX.Length;
        var shouldBeRemoved = fullFlagName.ToLower().EndsWith(FLAG_SUFFIX.ToLower())
                              && fullFlagName.Length > suffixLength;
        if (shouldBeRemoved)
        {
            return fullFlagName.Substring(0, fullFlagName.Length - suffixLength);
        }

        return fullFlagName;
    }

    private static string splitOnPascalCaseAndAddHyphens(string name)
    {
        return name.SplitPascalCase().Split(' ').Join("-");
    }
}