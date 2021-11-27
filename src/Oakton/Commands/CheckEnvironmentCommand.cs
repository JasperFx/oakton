using System;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Oakton.Environment;
using Spectre.Console;

namespace Oakton.Commands
{
    public class CheckEnvironmentInput : NetCoreInput
    {
        [Description("Use to optionally write the results of the environment checks to a file")]
        public string FileFlag { get; set; }
    }
    
    [Description("Execute all environment checks against the application", Name = "check-env")]
    public class CheckEnvironmentCommand : OaktonAsyncCommand<CheckEnvironmentInput>
    {
        public override async Task<bool> Execute(CheckEnvironmentInput input)
        {
            
            AnsiConsole.Write(
                new FigletText("Oakton")
                    .LeftAligned());


            using var host = input.BuildHost();
            var results = await EnvironmentChecker.ExecuteAllEnvironmentChecks(host.Services);

            if (input.FileFlag.IsNotEmpty())
            {
                results.WriteToFile(input.FileFlag);
                Console.WriteLine("Writing environment checks to " + input.FileFlag);
            }

            if (results.Failures.Any())
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Some environment checks failed!");
                return false;
            }

            ConsoleWriter.Write(ConsoleColor.Green, "All environment checks are good!");
            return true;
        }

    }
}