using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Baseline;
using Shouldly;
using Xunit;

namespace Oakton.Testing
{
    public class CommandExecutorTester
    {
        private readonly StringWriter theOutput = new StringWriter();


#if NET451
        private string directory = AppDomain.CurrentDomain.BaseDirectory;
#else
        private string directory = AppContext.BaseDirectory;
#endif
        private CommandExecutor executor;


        public CommandExecutorTester()
        {
            Console.SetOut(theOutput);

            executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommands(GetType().GetTypeInfo().Assembly);
            });
        }


        [Fact]
        public void execute_happy_path()
        {
            executor.Execute("say-name Lebron James")
                .ShouldBe(0);

            theOutput.ToString().ShouldContain("Lebron James");
        }

        [Fact]
        public void execute_async_happy_path()
        {
            executor.Execute("say-async-name Lebron James")
                .ShouldBe(0);

            theOutput.ToString().ShouldContain("Lebron James");
        }

        [Fact]
        public void no_command_argument_should_display_the_help()
        {
            executor.Execute("").ShouldBe(0);

            theOutput.ToString().ShouldContain("Available commands:");
            theOutput.ToString().ShouldContain("say-name -> Say my name");
        }

        [Fact]
        public void show_help_for_a_single_command()
        {
            executor.Execute("help say-name").ShouldBe(1);

            theOutput.ToString().ShouldContain("Usages for 'say-name' (Say my name)");
        }

        [Fact]
        public void run_a_command_that_fails()
        {
            executor.Execute("throwup").ShouldBe(1);

            theOutput.ToString().ShouldContain("DivideByZeroException");
        }

        [Fact]
        public void run_an_async_command_that_fails()
        {
            executor.Execute("throwupasync").ShouldBe(1);

            theOutput.ToString().ShouldContain("DivideByZeroException");
        }

        [Fact]
        public void run_with_options_if_the_options_file_does_not_exist()
        {
            executor.OptionsFile = "exec.opts";

            executor.Execute("say-name Lebron James")
                .ShouldBe(0);

            theOutput.ToString().ShouldContain("Lebron James");
        }

        [Fact]
        public void use_options_file_if_it_exists()
        {
            var path = directory.AppendPath("good.opts");
            new FileSystem().WriteStringToFile(path, "say-name Klay Thompson");

            executor.OptionsFile = "good.opts";

            executor.Execute("")
                .ShouldBe(0);

            theOutput.ToString().ShouldContain("Klay Thompson");
        }

        [Fact]
        public void can_set_flags_in_combination_with_opts()
        {
            var path = directory.AppendPath("override.opts");
            new FileSystem().WriteStringToFile(path, "option -b -n 1");

            executor.OptionsFile = "override.opts";

            executor.Execute("--number 5").ShouldBe(0);

            theOutput.ToString().ShouldContain("Big is true, Number is 5");
        }

        [Fact]
        public void execute_single_command_synchronously()
        {
            CommandExecutor.ExecuteCommand<OptionCommand>(new[] {"--big", "--number", "6"})
                .ShouldBe(0);

            theOutput.ToString().Trim().ShouldBe("Big is True, Number is 6");
        }

        [Fact]
        public async Task execute_single_command_asynchronously()
        {
            (await CommandExecutor.ExecuteCommandAsync<OptionCommand>(new[] { "--big", "--number", "7" }))
                .ShouldBe(0);

            theOutput.ToString().Trim().ShouldBe("Big is True, Number is 7");
        }
    }

    public class OptionInputs
    {
        public bool BigFlag;
        public int NumberFlag;
    }

    public class OptionCommand : OaktonCommand<OptionInputs>
    {
        public override bool Execute(OptionInputs input)
        {
            Console.WriteLine($"Big is {input.BigFlag}, Number is {input.NumberFlag}");

            return true;
        }
    }


    public class SayName
    {
        public string FirstName;
        public string LastName;
    }

    [Description("Say my name", Name = "say-name")]
    public class SayNameCommand : OaktonCommand<SayName>
    {
        public SayNameCommand()
        {
            Usage("Capture the users name").Arguments(x => x.FirstName, x => x.LastName);
        }

        public override bool Execute(SayName input)
        {
            Console.WriteLine($"{input.FirstName} {input.LastName}");
            return true;
        }
    }

    [Description("Say my name", Name = "say-async-name")]
    public class AsyncSayNameCommand : OaktonAsyncCommand<SayName>
    {
        public AsyncSayNameCommand()
        {
            Usage("Capture the users name").Arguments(x => x.FirstName, x => x.LastName);
        }

        public override async Task<bool> Execute(SayName input)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"{input.FirstName} {input.LastName}");
            });

            return true;
        }
    }

    public class ThrowUp
    {
    }

    public class ThrowUpCommand : OaktonCommand<ThrowUp>
    {
        public override bool Execute(ThrowUp input)
        {
            throw new DivideByZeroException("I threw up!");
        }
    }

    public class ThrowUpAsyncCommand : OaktonAsyncCommand<ThrowUp>
    {
        public override Task<bool> Execute(ThrowUp input)
        {
            throw new DivideByZeroException("I threw up!");
        }
    }
}
