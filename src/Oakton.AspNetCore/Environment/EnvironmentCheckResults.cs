using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;

namespace Oakton.AspNetCore.Environment
{
    public class EnvironmentCheckResults
    {
        private readonly IList<string> _successes = new List<string>();
        private readonly IList<EnvironmentFailure> _failures = new List<EnvironmentFailure>();

        public void RegisterSuccess(string description)
        {
            _successes.Add(description);
        }

        public void RegisterFailure(string description, Exception ex)
        {
            _failures.Add(new EnvironmentFailure(description, ex));
        }

        public string[] Successes => _successes.ToArray();

        public EnvironmentFailure[] Failures => _failures.ToArray();

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