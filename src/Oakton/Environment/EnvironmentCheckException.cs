using System;
using System.Linq;

namespace Oakton.Environment
{
    public class EnvironmentCheckException : AggregateException
    {
        public EnvironmentCheckResults Results { get; }

        public EnvironmentCheckException(EnvironmentCheckResults results) : base(results.ToString(), results.Failures.Select(x => x.Exception))
        {
            Results = results;
        }
    }
}