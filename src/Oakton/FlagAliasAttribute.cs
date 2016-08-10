using System;

namespace Oakton
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FlagAliasAttribute : Attribute
    {
        private readonly string _longAlias;
        private readonly char? _oneLetterAlias;

        public FlagAliasAttribute(string longAlias, char oneLetterAlias)
        {
            _longAlias = longAlias;
            _oneLetterAlias = oneLetterAlias;
        }

        public FlagAliasAttribute(char oneLetterAlias)
        {
            _oneLetterAlias = oneLetterAlias;
        }

        public FlagAliasAttribute(string longAlias)
        {
            _longAlias = longAlias;
        }

        public string LongAlias
        {
            get { return _longAlias; }
        }

        public char? OneLetterAlias
        {
            get { return _oneLetterAlias; }
        }
    }
}