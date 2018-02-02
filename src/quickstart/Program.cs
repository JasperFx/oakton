using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Oakton;

namespace quickstart
{
    // SAMPLE: Quickstart.Program1
    class Program
    {
        static int Main(string[] args)
        {
            // As long as this doesn't blow up, we're good to go
            return CommandExecutor.ExecuteCommand<NameCommand>(args);
        }
    }
    // ENDSAMPLE

    // SAMPLE: NameInput
    public class NameInput
    {
        [Description("The name to be printed to the console output")]
        public string Name { get; set; }

        [Description("The color of the text. Default is black")]
        public ConsoleColor Color { get; set; } = ConsoleColor.Black;

        [Description("Optional title preceeding the name")]
        public string TitleFlag { get; set; }
    }
    // ENDSAMPLE

    // SAMPLE: NameCommand
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

            // This is a little helper in Oakton for getting
            // cute with colors in the console output
            ConsoleWriter.Write(input.Color, text);


            // Just telling the OS that the command
            // finished up okay
            return true;
        }
    }
    // ENDSAMPLE


    public class OtherNameCommand : OaktonCommand<NameInput>
    {
        // SAMPLE: specifying-usages
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
        // ENDSAMPLE

        public override bool Execute(NameInput input)
        {
            return true;
        }
    }

    // SAMPLE: command-alias
    [Description("Say my name differently", Name = "different-name")]
    public class AliasedCommand : OaktonCommand<NameInput>
    // ENDSAMPLE
    {
        public override bool Execute(NameInput input)
        {
            throw new NotImplementedException();
        }
    }

    // SAMPLE: async-command
    public class DoNameThingsCommand : OaktonAsyncCommand<NameInput>
    {
        public override async Task<bool> Execute(NameInput input)
        {
            ConsoleWriter.Write(input.Color, "Starting...");
            await Task.Delay(TimeSpan.FromSeconds(3));

            ConsoleWriter.Write(input.Color, $"Done! Hello {input.Name}");
            return true;
        }
    }
    // ENDSAMPLE
}
