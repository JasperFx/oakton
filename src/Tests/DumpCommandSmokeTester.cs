using System;
using System.Linq;
using System.Reflection;
using Baseline;
using Oakton;
using Oakton.Reporting;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class DumpCommandSmokeTester
    {
        public DumpCommandSmokeTester()
        {
            theFactory = new CommandFactory();
            theFactory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            theInput = new DumpUsagesInput
            {
                ApplicationName = "ripple",
                Location = "somewhere.xml",
                Commands = theFactory
            };

            theCommand = new DumpUsagesCommand();

            report = theCommand.BuildReport(theInput);

            commandReport = report.Commands.Single(x => x.Name == "crazy");
        }


        private CommandFactory theFactory;
        private DumpUsagesInput theInput;
        private DumpUsagesCommand theCommand;
        private CommandLineApplicationReport report;
        private CommandReport commandReport;

        [Fact]
        public void can_build_a_report()
        {

            report.ShouldNotBeNull();
            report.Commands.Any().ShouldBeTrue();
        }

        [Fact]
        public void spot_check_a_command()
        {
            commandReport.Description.ShouldBe("The crazy command");
            commandReport.Name.ShouldBe("crazy");
            

        }

        [Fact]
        public void spot_check_args()
        {
            commandReport.Arguments.Select(x => x.Name)
    .ShouldHaveTheSameElementsAs("arg1", "arg2", "arg3");

            var arg1 = commandReport.Arguments.Single(x => x.Name == "arg1");
            arg1.Name.ShouldBe("arg1");
            arg1.Description.ShouldBe("The first arg");
        }

        [Fact]
        public void spot_check_flags()
        {
            commandReport.Flags.Select(x => x.Description)
                .ShouldHaveTheSameElementsAs("make it red", "make it blue", "make it green");

            commandReport.Flags.Select(x => x.UsageDescription)
                .ShouldHaveTheSameElementsAs("[-r, --red]", "[-b, --blue]", "[-g, --green]");
        }

        [Fact]
        public void spot_check_usages()
        {
            commandReport.Usages.Select(x => x.Description)
                .ShouldHaveTheSameElementsAs("Only one", "only two", "All");

            commandReport.Usages.First().Usage
                .ShouldBe("ripple crazy <arg1> [-r, --red] [-b, --blue] [-g, --green]");
        }

        [Fact]
        public void dump_the_file()
        {
            theCommand.Execute(theInput).ShouldBeTrue();

            var report = new FileSystem().LoadFromFile<CommandLineApplicationReport>(theInput.Location);

            report.ShouldNotBeNull();
            report.Commands.Any().ShouldBeTrue();
        }

        [Fact]
        public void can_read_usage_for_a_single_usage_command()
        {
            var simpleReport = report.Commands.Single(x => x.Name == "simple");
            simpleReport.Usages.Count().ShouldBe(1);
        }
    }

    public class CrazyInput
    {
        [Description("The first arg")]
        public string Arg1 { get; set; }

        [Description("The second arg")]
        public string Arg2 { get; set; }

        [Description("The third arg")]
        public string Arg3 { get; set; }

        [Description("make it red")]
        public bool RedFlag { get; set; }
        [Description("make it blue")]
        public bool BlueFlag { get; set; }
        [Description("make it green")]
        public bool GreenFlag { get; set; }
    }

    [Description("The crazy command", Name = "crazy")]
    public class CrazyCommand : OaktonCommand<CrazyInput>
    {
        public CrazyCommand()
        {
            Usage("Only one").Arguments(x => x.Arg1);
            Usage("only two").Arguments(x => x.Arg1, x => x.Arg2);
            Usage("All").Arguments(x => x.Arg1, x => x.Arg2, x => x.Arg3);
        }

        public override bool Execute(CrazyInput input)
        {
            throw new NotImplementedException();
        }
    }

    [Description("The simple one")]
    public class SingleCommand : OaktonCommand<CrazyInput>
    {
        public override bool Execute(CrazyInput input)
        {
            throw new NotImplementedException();
        }
    }
}