using System;
using System.Linq;
using Spectre.Console;

namespace Oakton.Help;

#nullable disable annotations // FIXME

[Description("List all the available commands", Name = "help")]
public class HelpCommand : OaktonCommand<HelpInput>
{
    public HelpCommand()
    {
        Usage("List all the available commands").Arguments(x => x.Name);
        Usage("Show all the valid usages for a command");
    }

    public override bool Execute(HelpInput input)
    {
        if (input.Usage != null)
        {
            input.Usage.WriteUsages(input.AppName);
            return false;
        }

        if (input.InvalidCommandName)
        {
            writeInvalidCommand(input.Name);
            listAllCommands(input);
            return false;
        }

        listAllCommands(input);
        return true;
    }

    private void listAllCommands(HelpInput input)
    {
        if (!input.CommandTypes.Any())
        {
            Console.WriteLine("There are no known commands in this executable!");
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]The available commands are:[/]");

        var table = new Table
        {
            Border = TableBorder.SimpleHeavy
        };

        table.AddColumns("Alias", "Description");
        foreach (var type in input.CommandTypes.OrderBy(CommandFactory.CommandNameFor))
            table.AddRow(CommandFactory.CommandNameFor(type), CommandFactory.DescriptionFor(type));

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "Use [italic]dotnet run -- ? [[command name]][/] or [italic]dotnet run -- help [[command name]][/] to see usage help about a specific command");
    }

    private void writeInvalidCommand(string commandName)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[red]'{commandName}' is not a command.  See available commands.[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
    }
}