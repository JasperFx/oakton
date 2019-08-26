using System;
using System.Linq;

namespace Oakton.AspNetCore.Environment
{
    public class EnvironmentCheckException : AggregateException
    {
        public EnvironmentCheckException(EnvironmentCheckResults results) : base(results.ToString(), results.Failures.Select(x => x.Exception))
        {
        }
    }
}