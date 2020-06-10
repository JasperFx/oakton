using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Baseline;
using Baseline.Conversion;
using Baseline.Reflection;

namespace Oakton.Parsing
{
    public static class InputParser
    {
        private static readonly string LONG_FLAG_PREFIX = "--";
        private static readonly Regex LONG_FLAG_REGEX = new Regex("^{0}[^-]+".ToFormat(LONG_FLAG_PREFIX));
        
        private static readonly string SHORT_FLAG_PREFIX = "-";
        private static readonly Regex SHORT_FLAG_REGEX = new Regex("^{0}[^-]+".ToFormat(SHORT_FLAG_PREFIX)); 
        
        private static readonly string FLAG_SUFFIX = "Flag";
        private static readonly Conversions _converter = new Conversions();


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
                
                if (memberType != typeof (string) && memberType.Closes(typeof (IEnumerable<>)))
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
            return  IsShortFlag(token) || IsLongFlag(token);
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
            var name = member.Name;
            if (name.EndsWith("Flag") && name.Length > 4)
            {
                name = name.Substring(0, member.Name.Length - 4);
            }

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
                           ShortForm = (SHORT_FLAG_PREFIX + oneLetterName),
                           LongForm = LONG_FLAG_PREFIX + name.ToLower(),
                           LongFormOnly = longFormOnly
                       };
        }

        private static string splitOnPascalCaseAndAddHyphens(string name)
        {
            return name.SplitPascalCase().Split(' ').Join("-");
        }
    }
}