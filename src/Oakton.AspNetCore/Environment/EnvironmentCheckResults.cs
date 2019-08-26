using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton.AspNetCore.Environment
{
    public class EnvironmentCheckResults
    {
        public void RegisterSuccess(string description)
        {
            Successes.Add(description);
        }

        public void RegisterFailure(string description, Exception ex)
        {
            Failures.Add(new EnvironmentFailure(description, ex));
        }

        public IList<string> Successes { get; } = new List<string>();

        public IList<EnvironmentFailure> Failures { get; } = new List<EnvironmentFailure>();

        public bool Succeeded()
        {
            return !Failures.Any();
        }

        public void Assert()
        {
            if (Failures.Any())
            {
                throw new EnvironmentCheckException(this);
            }
        }

        public override string ToString()
        {
            return $"Environment check failures:\n{Failures.Select(x => x.Description).Join("\n")}";
        }

        public class EnvironmentFailure
        {
            public string Description { get; }
            public Exception Exception { get; }

            public EnvironmentFailure(string description, Exception exception)
            {
                Description = description;
                Exception = exception;
            }
        }
    }
}