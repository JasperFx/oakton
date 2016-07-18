namespace Oakton
{
    public class FlagAliases
    {
        public string LongForm { get; set; }
        public string ShortForm { get; set; }

        public bool Matches(string token)
        {
            if(InputParser.IsShortFlag(token))
            {
                return token == ShortForm;
            }

            var lowerToken = token.ToLower();

            return lowerToken == LongForm.ToLower();
        }

        public override string ToString()
        {
            return $"{ShortForm}, {LongForm}";
        }
    }
}