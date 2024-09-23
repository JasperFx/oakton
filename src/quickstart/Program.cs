using System;
using System.Threading.Tasks;
using Oakton;
using Spectre.Console;

namespace quickstart
{
    #region sample_Quickstart.Program1
    class Program
    {
        static int Main(string[] args)
        {
            // As long as this doesn't blow up, we're good to go
            return CommandExecutor.ExecuteCommand<NameCommand>(args);
        }
    }
    #endregion

    #region sample_NameInput
    public class NameInput
    {
        [Description("The name to be printed to the console output")]
        public string Name { get; set; }

        [Description("The color of the text. Default is black")]
        public ConsoleColor Color { get; set; } = ConsoleColor.Black;

        [Description("Optional title preceeding the name")]
        public string TitleFlag { get; set; }
    }
    #endregion

    #region sample_NameCommand
    [Description("Print somebody's name")]
    public class NameCommand : OaktonCommand<NameInput>
    {
        public NameCommand()
        {
            // The usage pattern definition here is completely
            // optional
            Usage("Default Color").Arguments(x => x.Name);
            Usage("Print name with specified color").Arguments(x => x.Name, x => x.Color);
        }

        public override bool Execute(NameInput input)
        {
            var text = input.Name;
            if (!string.IsNullOrEmpty(input.TitleFlag))
            {
                text = input.TitleFlag + " " + text;
            }

            AnsiConsole.MarkupLine($"[{input.Color}]{text}[/]");

            // Just telling the OS that the command
            // finished up okay
            return true;
        }
    }
    #endregion


    public class OtherNameCommand : OaktonCommand<NameInput>
    {
        #region sample_specifying_usages
        public OtherNameCommand()
        {
            // You can specify multiple usages
            Usage("describe what is different about this usage")
                // Specify which arguments are part of this usage
                // and in what order they should be expressed
                // by the user
                .Arguments(x => x.Name, x => x.Color)

                // Optionally, you can provide a white list of valid
                // flags in this usage
                .ValidFlags(x => x.TitleFlag);
        }
        #endregion

        public override bool Execute(NameInput input)
        {
            return true;
        }
    }

    #region sample_command_alias
    [Description("Say my name differently", Name = "different-name")]
    public class AliasedCommand : OaktonCommand<NameInput>
    #endregion
    {
        public override bool Execute(NameInput input)
        {
            throw new NotImplementedException();
        }
    }

    #region sample_async_command
    public class DoNameThingsCommand : OaktonAsyncCommand<NameInput>
    {
        public override async Task<bool> Execute(NameInput input)
        {
            AnsiConsole.MarkupLine($"[{input.Color}]Starting...[/]");
            await Task.Delay(TimeSpan.FromSeconds(3));

            AnsiConsole.MarkupLine($"[{input.Color}]Done! Hello {input.Name}[/]");
            return true;
        }
    }
    #endregion
}
