using System;
using System.Collections.Generic;

namespace Oakton.Help;

#nullable disable annotations // FIXME

public class HelpInput
{
    [IgnoreOnCommandLine] public IEnumerable<Type> CommandTypes { get; set; }

    [Description("A command name")] public string Name { get; set; }

    [IgnoreOnCommandLine] public bool InvalidCommandName { get; set; }

    [IgnoreOnCommandLine] public UsageGraph Usage { get; set; }

    [IgnoreOnCommandLine] public string AppName { get; set; } = "dotnet run --";
}