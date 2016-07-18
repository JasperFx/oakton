using System.Linq;
using Baseline;
using Oakton.Reporting;

namespace Oakton
{
    public class DumpUsagesInput
    {
        public string ApplicationName { get; set; }
        public string Location { get; set; }

        [IgnoreOnCommandLine]
        public ICommandFactory Commands { get; set; }
    }

    public class DumpUsagesCommand : OaktonCommand<DumpUsagesInput>
    {
        public override bool Execute(DumpUsagesInput input)
        {
            var report = BuildReport(input);

            new FileSystem().WriteObjectToFile(input.Location, report);

            return true;
        }

        public CommandLineApplicationReport BuildReport(DumpUsagesInput input)
        {
            var report = new CommandLineApplicationReport
            {
                ApplicationName = input.ApplicationName,
                Commands = input.Commands.BuildAllCommands().Select(x => ToReport(x, input.ApplicationName)).ToArray()
            };
            return report;
        }

        public CommandReport ToReport(IOaktonCommand command, string applicationName)
        {
            return command.Usages.ToReport(applicationName);
        }
    }


}