using System;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     Use to override the long and/or short flag keys of a property or field
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FlagAliasAttribute : Attribute
{
    public FlagAliasAttribute(string longAlias, char oneLetterAlias)
    {
        LongAlias = longAlias;
        OneLetterAlias = oneLetterAlias;
    }

    public FlagAliasAttribute(char oneLetterAlias)
    {
        OneLetterAlias = oneLetterAlias;
    }

    public FlagAliasAttribute(string longAlias)
    {
        LongAlias = longAlias;
    }

    public FlagAliasAttribute(string longAlias, bool longAliasOnly)
    {
        LongAlias = longAlias;
        LongAliasOnly = longAliasOnly;
    }

    public string LongAlias { get; }


    public char? OneLetterAlias { get; }

    public bool LongAliasOnly { get; }
}