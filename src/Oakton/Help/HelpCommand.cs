using System;
using System.Linq;
using Baseline;

namespace Oakton.Help
{
    [Description("list all the available commands", Name = "help")]
    public class HelpCommand : OaktonCommand<HelpInput>
    {
        public HelpCommand()
        {
            Usage("List all the available commands").Arguments(x => x.Name);
            Usage("Show all the valid usages for a command");
        }

        // TODO -- have it write out its own usage
        // TODO -- look for command line stuff
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

            var report = new TwoColumnReport("Available commands:");
            input.CommandTypes.OrderBy(CommandFactory.CommandNameFor).Each(type =>
            {
                report.Add(CommandFactory.CommandNameFor(type), CommandFactory.DescriptionFor(type));
            });

            report.Write();
        }

        private void writeInvalidCommand(string commandName)
        {
            ConsoleWriter.Line();
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleWriter.Write("'{0}' is not a command.  See available commands.", commandName);
            Console.ResetColor();
            ConsoleWriter.Line();
            ConsoleWriter.Line();
        }
    }
}