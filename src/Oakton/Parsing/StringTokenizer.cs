using System.Collections.Generic;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public static class StringTokenizer
{
    public static IEnumerable<string> Tokenize(string content)
    {
        var searchString = content.Trim();
        if (searchString.Length == 0)
        {
            return new string[0];
        }

        var parser = new TokenParser();

        foreach (var c in content.ToCharArray()) parser.Read(c);

        // Gotta force the parser to know it's done
        parser.Read('\n');

        return parser.Tokens;
    }
}