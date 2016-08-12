using System;
using System.IO;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Oakton.Testing
{
    public class CommandExecutorTester
    {
        private readonly StringWriter theOutput = new StringWriter();

        public CommandExecutorTester()
        {
            Console.SetOut(theOutput);


        }

        [Fact]
        public void execute_happy_path()
        {
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommands(GetType().GetTypeInfo().Assembly);
            });

            executor.Execute("say-name Lebron James")
                .ShouldBe(0);

            theOutput.ToString().ShouldContain("Lebron James");
        }

        [Fact]
        public void no_command_argument_should_display_the_help()
        {
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommands(GetType().GetTypeInfo().Assembly);
            });

            executor.Execute("").ShouldBe(0);

            theOutput.ToString().ShouldContain("Available commands:");
            theOutput.ToString().ShouldContain("say-name -> Say my name");
        }

        [Fact]
        public void show_help_for_a_single_command()
        {
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommands(GetType().GetTypeInfo().Assembly);
            });

            executor.Execute("help say-name").ShouldBe(1);

           

            theOutput.ToString().ShouldContain("Usages for 'say-name' (Say my name)");
        }

        [Fact]
        public void run_a_command_that_fails()
        {
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommands(GetType().GetTypeInfo().Assembly);
            });

            executor.Execute("throwup").ShouldBe(1);

            theOutput.ToString().ShouldContain("DivideByZeroException");
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
}