using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JasperFx.Core;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

public class EnvironmentCheckResults
{
    private readonly IList<EnvironmentFailure> _failures = new List<EnvironmentFailure>();
    private readonly IList<string> _successes = new List<string>();

    public string[] Successes => _successes.ToArray();

    public EnvironmentFailure[] Failures => _failures.ToArray();

    public void RegisterSuccess(string description)
    {
        _successes.Add(description);
    }

    public void RegisterFailure(string description, Exception ex)
    {
        _failures.Add(new EnvironmentFailure(description, ex));
    }

    /// <summary>
    ///     Did all the environment checks succeed?
    /// </summary>
    /// <returns></returns>
    public bool Succeeded()
    {
        return !Failures.Any();
    }

    /// <summary>
    ///     Throw an explanatory exception if there were any failed environment
    ///     checks
    /// </summary>
    /// <exception cref="EnvironmentCheckException"></exception>
    public void Assert()
    {
        if (Failures.Any())
        {
            throw new EnvironmentCheckException(this);
        }
    }

    public override string ToString()
    {
        return $"Environment check failures:\n{Failures.Select(x => x.Description).Join(System.Environment.NewLine)}";
    }

    public void WriteToFile(string file)
    {
        var writer = new StringWriter();
        var status = Succeeded() ? "Success" : "Failed";
        writer.WriteLine($"Environment Checks at {DateTime.Now.ToShortTimeString()} ({status})");

        writer.WriteLine();

        if (Successes.Any())
        {
            writer.WriteLine("=================================================================");
            writer.WriteLine("=                    Successful Checks                          =");
            writer.WriteLine("=================================================================");

            foreach (var resultsSuccess in Successes) writer.WriteLine("* " + resultsSuccess);

            writer.WriteLine();
        }

        if (Failures.Any())
        {
            writer.WriteLine("=================================================================");
            writer.WriteLine("=                          Failures                             =");
            writer.WriteLine("=================================================================");

            foreach (var failure in Failures)
            {
                writer.WriteLine("Failure: " + failure.Description);
                writer.WriteLine(failure.Exception.ToString());
                writer.WriteLine();
                writer.WriteLine();
            }
        }

        File.WriteAllText(file, writer.ToString());
    }

    public class EnvironmentFailure
    {
        public EnvironmentFailure(string description, Exception exception)
        {
            Description = description;
            Exception = exception;
        }

        public string Description { get; }
        public Exception Exception { get; }
    }
}