using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Baseline;
using Baseline.Conversion;
using Baseline.Reflection;

namespace Oakton
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
            return inputType.GetProperties()
                .Where(prop => prop.CanWrite)
                .Where(prop => !prop.HasAttribute<IgnoreOnCommandLineAttribute>())
                .Select(BuildHandler).ToList();
        }

        public static ITokenHandler BuildHandler(PropertyInfo property)
        {
            if (!property.Name.EndsWith(FLAG_SUFFIX))
            {
                if (property.PropertyType != typeof (string) && property.PropertyType.Closes(typeof (IEnumerable<>)))
                {
                    return new EnumerableArgument(property, _converter);
                }

                return new Argument(property, _converter);
            }


            if (property.PropertyType != typeof(string) && property.PropertyType.Closes(typeof(IEnumerable<>)))
            {
                return new EnumerableFlag(property, _converter);
            }

            if (property.PropertyType == typeof(bool))
            {
                return new BooleanFlag(property);
            }
            
                //if for enumerable too

                //else
                return new Flag(property, _converter);
                
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

        public static bool IsFlagFor(string token, PropertyInfo property)
        {
            return ToFlagAliases(property).Matches(token);
        }

        public static FlagAliases ToFlagAliases(PropertyInfo property)
        {
            var name = property.Name;
            if (name.EndsWith("Flag"))
            {
                name = name.Substring(0, property.Name.Length - 4);
            }

            name = splitOnPascalCaseAndAddHyphens(name);

            var oneLetterName = name.ToLower()[0];

            property.ForAttribute<FlagAliasAttribute>(att =>
                                                          {
                                                              name = att.LongAlias ?? name;
                                                              oneLetterName = att.OneLetterAlias ?? oneLetterName;
                                                          });
            return new FlagAliases
                       {
                           ShortForm = (SHORT_FLAG_PREFIX + oneLetterName),
                           LongForm = LONG_FLAG_PREFIX + name.ToLower()
                       };
        }

        private static string splitOnPascalCaseAndAddHyphens(string name)
        {
            return name.SplitPascalCase().Split(' ').Join("-");
        }
    }
}