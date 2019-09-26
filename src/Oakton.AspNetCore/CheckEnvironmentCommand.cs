using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Oakton.AspNetCore.Environment;

namespace Oakton.AspNetCore
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
            using (var host = input.BuildHost())
            {
                var results = await EnvironmentChecker.ExecuteAllEnvironmentChecks(host.Services);

                if (input.FileFlag.IsNotEmpty())
                {
                    results.WriteToFile(input.FileFlag);
                    Console.WriteLine("Writing environment checks to " + input.FileFlag);
                }
                
                results.Assert();
                
                ConsoleWriter.Write(ConsoleColor.Green, "All environment checks are good!");
            }

            return true;
        }

    }
}