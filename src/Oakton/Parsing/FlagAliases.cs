namespace Oakton.Parsing;

public class FlagAliases
{
    public string LongForm { get; set; }
    public string ShortForm { get; set; }

    public bool LongFormOnly { get; set; }

    public bool Matches(string token)
    {
        if (!LongFormOnly && InputParser.IsShortFlag(token))
        {
            return token == ShortForm;
        }

        var lowerToken = token.ToLower();

        return lowerToken == LongForm.ToLower();
    }

    public override string ToString()
    {
        return LongFormOnly ? $"{LongForm}" : $"{ShortForm}, {LongForm}";
    }
}