using System.Collections.Generic;
using System.Linq;
using JasperFx.Core;
using Oakton.Parsing;
using Spectre.Console;

namespace Oakton.Help;

#nullable disable annotations // FIXME

public class CommandUsage
{
    public string Description { get; set; }
    public IEnumerable<Argument> Arguments { get; set; }
    public IEnumerable<ITokenHandler> ValidFlags { get; set; }

    public string ToUsage(string appName, string commandName)
    {
        var arguments = Arguments.Union(ValidFlags)
            .Select(x => x.ToUsageDescription())
            .Join(" ");

        return $"{appName} {commandName} {arguments}";
    }

    public void WriteUsage(string appName, string commandName)
    {
        var arguments = Arguments.Union(ValidFlags)
            .Select(x => x.ToUsageDescription())
            .Join(" ");

        AnsiConsole.MarkupLine($"[bold]{appName}[/] [bold]{commandName}[/] [cyan][{arguments}][/]");
    }


    public bool IsValidUsage(IEnumerable<ITokenHandler> handlers)
    {
        var actualArgs = handlers.OfType<Argument>();
        if (actualArgs.Count() != Arguments.Count())
        {
            return false;
        }

        if (!Arguments.All(x => actualArgs.Contains(x)))
        {
            return false;
        }

        var flags = handlers.Where(x => !(x is Argument));
        return flags.All(x => ValidFlags.Contains(x));
    }
}