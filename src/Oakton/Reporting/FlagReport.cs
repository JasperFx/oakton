using Oakton.Parsing;

namespace Oakton.Reporting
{
    public class FlagReport
    {
        public FlagReport()
        {
        }

        public FlagReport(ITokenHandler token)
        {
            UsageDescription = token.ToUsageDescription();
            Description = token.Description;
        }

        public string UsageDescription { get; set; }
        public string Description { get; set; }
    }
}