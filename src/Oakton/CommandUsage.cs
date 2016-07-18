using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton
{
    public class CommandUsage
    {
        public string Description { get; set; }
        public IEnumerable<Argument> Arguments { get; set; }
        public IEnumerable<ITokenHandler> ValidFlags { get; set; }

        public UsageReport ToReport(string appName, string commandName)
        {
            return new UsageReport
            {
                Description = Description,
                Usage = ToUsage(appName, commandName)
            };
        }

        public string ToUsage(string appName, string commandName)
        {
            var arguments = Arguments.Union(ValidFlags)
                .Select(x => x.ToUsageDescription())
                .Join(" ");

            return $"{appName} {commandName} {arguments}";
        }


        public bool IsValidUsage(IEnumerable<ITokenHandler> handlers)
        {
            var actualArgs = handlers.OfType<Argument>();
            if (actualArgs.Count() != Arguments.Count()) return false;

            if (!Arguments.All(x => actualArgs.Contains(x)))
            {
                return false;
            }

            var flags = handlers.Where(x => !(x is Argument));
            return flags.All(x => ValidFlags.Contains(x));
        }
    }
}