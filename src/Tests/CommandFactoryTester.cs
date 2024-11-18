using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ExtensionCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using Oakton.Help;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class CommandFactoryTester
    {
        [Fact]
        public void get_the_command_name_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (MyCommand)).ShouldBe("my");
        }

        [Fact]
        public void get_the_command_name_for_a_class_not_ending_in_command()
        {
            CommandFactory.CommandNameFor(typeof(SillyCommand)).ShouldBe("silly");
        }

        [Fact]
        public void get_the_command_name_for_a_class_that_has_a_longer_name()
        {
            CommandFactory.CommandNameFor(typeof(RebuildAuthorizationCommand)).ShouldBe("rebuildauthorization");
        }

        [Fact]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (DecoratedCommand)).ShouldBe("this");
        }

        [Fact]
        public void get_the_description_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (MyCommand)).ShouldBe(typeof (MyCommand).FullName);
        }

        [Fact]
        public void get_the_description_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (My2Command)).ShouldBe("something");
        }

        [Fact]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute_but_without_the_name_specified()
        {
            CommandFactory.CommandNameFor(typeof (My2Command)).ShouldBe("my2");
        }

        [Fact]
        public void build()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.Build("my").ShouldBeOfType<MyCommand>();
            factory.Build("my2").ShouldBeOfType<My2Command>();
            factory.Build("this").ShouldBeOfType<DecoratedCommand>();
        }

        [Fact]
        public void trying_to_build_a_missing_command_will_list_the_existing_commands()
        {
            var factory = new CommandFactory();
            factory.SetAppName("bottles");
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var commandRun = factory.BuildRun("junk");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();

            theInput.AppName.ShouldBe("bottles");
            theInput.Name.ShouldBe("junk");
            theInput.InvalidCommandName.ShouldBeTrue();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Fact]
        public void build_help_command_with_valid_name_argument()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var commandRun = factory.BuildRun("help my");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();
            theInput.Name.ShouldBe("my");
            theInput.InvalidCommandName.ShouldBeFalse();
            theInput.Usage.ShouldNotBeNull();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Fact]
        public void build_help_command_with_invalid_name_argument()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var commandRun = factory.BuildRun("help junk");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();
            theInput.Name.ShouldBe("junk");
            theInput.InvalidCommandName.ShouldBeTrue();
            theInput.Usage.ShouldBeNull();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Fact]
        public void build_command_from_a_string()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.BuildRun("my Jeremy --force");

            run.Command.ShouldBeOfType<MyCommand>();
            var input = run.Input.ShouldBeOfType<MyCommandInput>();

            input.Name.ShouldBe("Jeremy");
            input.ForceFlag.ShouldBeTrue();
        }

        [Fact]
        public void call_through_the_configure_run()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            object input = null;
            object cmd = null;

            factory.ConfigureRun = r =>
            {
                cmd = r.Command;
                input = r.Input;
            };

            var run = factory.BuildRun("my Jeremy --force");

            cmd.ShouldBe(run.Command);
            input.ShouldBe(run.Input);
        }

        [Fact]
        public void fetch_the_help_command_run()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.HelpRun(new Queue<string>());
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Fact]
        public void fetch_the_help_command_if_the_args_are_empty()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.BuildRun(new string[0]);
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Fact]
        public void fetch_the_help_command_if_the_command_is_help()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.BuildRun(new [] {"help"});
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }


        [Fact]
        public void fetch_the_help_command_if_the_command_is_question_mark()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.BuildRun(new [] {"?"});
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Fact]
        public void smoke_test_the_writing()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);
            factory.HelpRun(new Queue<string>()).Execute();
        }

        [Fact]
        public void build_command_with_multiargs()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            var run = factory.BuildRun("my Jeremy -ft");
            var input = run.Input.ShouldBeOfType<MyCommandInput>();
            input.ForceFlag.ShouldBeTrue();
            input.SecondFlag.ShouldBeFalse();
            input.ThirdFlag.ShouldBeTrue();
        }

        [Fact]
        public void no_default_command_by_default()
        {
            var factory = new CommandFactory();
            factory.DefaultCommand.ShouldBeNull();

        }

        [Fact]
        public void default_command_is_derived_as_the_only_one()
        {
            var factory = new CommandFactory();
            factory.RegisterCommand<RebuildAuthorizationCommand>();

            factory.DefaultCommand.ShouldBe(typeof(RebuildAuthorizationCommand));
        }
        
        [Fact]
        public void default_command_is_derived_as_the_only_one_2()
        {
            var factory = new CommandFactory();
            factory.RegisterCommand(typeof(RebuildAuthorizationCommand));

            factory.DefaultCommand.ShouldBe(typeof(RebuildAuthorizationCommand));
        }

        [Fact]
        public void explicit_default_command()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(RebuildAuthorizationCommand);

            factory.DefaultCommand.ShouldBe(typeof(RebuildAuthorizationCommand));
        }


        [Fact]
        public void build_command_with_default_command_and_empty()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(NoArgCommand);

            factory.BuildRun("").Command.ShouldBeOfType<NoArgCommand>();
        }

        [Fact]
        public void build_command_with_default_command_and_first_arg_is_flag()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(OnlyFlagsCommand);

            var commandRun = factory.BuildRun("--verbose");
            commandRun.Command.ShouldBeOfType<OnlyFlagsCommand>();
            commandRun.Input.ShouldBeOfType<OnlyFlagsInput>()
                .VerboseFlag.ShouldBeTrue();
        }
        
        [Fact]
        public void build_command_with_default_command_and_first_arg_is_flag_that_is_not_valid()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(OnlyFlagsCommand);

            var commandRun = factory.BuildRun("--wrong");
            commandRun.Command.ShouldBeOfType<HelpCommand>();
            commandRun.Input.ShouldBeOfType<HelpInput>()
                .Name.ShouldBe("onlyflags");
        }
        
        [Fact]
        public void build_command_with_default_command_and_first_arg_for_the_default_command()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(SillyCommand);

            var commandRun = factory.BuildRun("blue");
            commandRun.Command.ShouldBeOfType<SillyCommand>();
            commandRun.Input.ShouldBeOfType<MyCommandInput>()
                .Name.ShouldBe("blue");
        }
        
        [Fact]
        public void still_use_help_with_default_command()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(RebuildAuthorizationCommand);

            factory.BuildRun("help").Command.ShouldBeOfType<HelpCommand>();
        }

        [Fact]
        public void build_the_default_command_using_arguments()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().GetTypeInfo().Assembly);

            factory.DefaultCommand = typeof(RebuildAuthorizationCommand);

            factory.BuildRun("Hank").Input
                .ShouldBeOfType<MyCommandInput>()
                .Name.ShouldBe("Hank");
        }

        [Fact]
        public void can_discover_extension_commands()
        {
            var factory = new CommandFactory();
            factory.RegisterCommandsFromExtensionAssemblies();

            factory.AllCommandTypes()
                .ShouldContain(typeof(ExtensionCommand));

            factory.AllCommandTypes()
                .ShouldContain(typeof(Extension2Command));

        }

        [Fact]
        public async Task set_up_command_prereq_using_beforebuild()
        {
          var factory = new CommandFactory();
          factory.RegisterCommands(GetType().GetTypeInfo().Assembly);
          factory.BeforeBuild = (commandName, input) => { ConfigureMe.IsSet = true; };

          var run = factory.BuildRun("depends-static");
          (await run.Execute()).ShouldBe(true);
        }

        [Fact]
        public async Task can_execute_command_constructor_custom_usage_all_optional_arguments()
        {
          var factory = new CommandFactory();
          factory.RegisterCommand<OptionalArgumentsCommand>();
          var run = factory.BuildRun("optionalarguments");
          (await run.Execute()).ShouldBe(true);
        }

        [Fact]
        public async Task can_execute_command_no_default_constructor()
        {
          var factory = new CommandFactory(new NoDefaultConstructorCommandCreator());
          factory.RegisterCommand<NoDefaultConstructorCommand>();
          var run = factory.BuildRun("nodefaultconstructor");
          (await run.Execute()).ShouldBe(true);
        }

        [Fact]
        public void discover_and_apply_extension_service()
        {
            var factory = new CommandFactory(new NoDefaultConstructorCommandCreator());
            
            // There's a marked extension type on this assembly
            factory.RegisterCommands(typeof(ExtensionInput).Assembly);

            var builder = Host.CreateDefaultBuilder();
            
            factory.ApplyExtensions(builder);

            using var host = builder.Build();
            var container = host.Services;

            container.GetRequiredService<IExtensionService>()
                .ShouldBeOfType<ExtensionService>();
        }
    }

    public class NulloInput
    {
        
    }

    public class NoArgCommand : OaktonCommand<NulloInput>
    {
        public override bool Execute(NulloInput input)
        {
            return true;
        }
    }

    public class RebuildAuthorizationCommand : OaktonCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class SillyCommand : OaktonCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class MyCommand : OaktonCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }


    }


    public class OnlyFlagsInput
    {
        public bool VerboseFlag { get; set; }
    }
    
    public class OnlyFlagsCommand : OaktonCommand<OnlyFlagsInput>
    {
        public override bool Execute(OnlyFlagsInput input)
        {
            return true;
        }
    }

    [Description("something")]
    public class My2Command : OaktonCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    [Description("this", Name = "this")]
    public class DecoratedCommand : OaktonCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    public static class ConfigureMe
    {
      public static bool IsSet { get; set; }
    }

    [Description("Depends on some static state, e.g. configuring DI container based on inputs.", Name="depends-static")]
    public class DependsOnStaticCommand : OaktonCommand<NulloInput>
    {
      public override bool Execute(NulloInput input)
      {
        return ConfigureMe.IsSet;
      }
    }


    public class MyCommandInput
    {
        public string Name { get; set; }
        public bool ForceFlag { get; set; }
        public bool SecondFlag { get; set; }
        public bool ThirdFlag { get; set; }
    }

    public class OptionalArgumentsInput
    {
      [Description("Listening on defined port")]
      public int Port { get; set; } = 12345;

      [Description("Get binaries"), FlagAlias("withBinaries", 'b')]
      public bool WithBinariesFlag { get; set; }
    }

    public class OptionalArgumentsCommand : OaktonCommand<OptionalArgumentsInput>
    {
      public OptionalArgumentsCommand()
      {
        Usage("Listen on default port").Arguments();
        Usage("Listen with explicit port").Arguments(i => i.Port);
      }

      public override bool Execute(OptionalArgumentsInput input)
      {
        return true;
      }
    }

    // Not exported to avoid breaking other tests that use assembly-wide command registration.
    internal class NoDefaultConstructorCommand : OaktonCommand<OptionalArgumentsInput>
    {
      public NoDefaultConstructorCommand(string _)
      {
        Usage("Listen on default port").Arguments();
      }

      public override bool Execute(OptionalArgumentsInput input)
      {
        return true;
      }
    }

    public class NoDefaultConstructorCommandCreator : ICommandCreator
    {
      public IOaktonCommand CreateCommand(Type commandType)
      {
        return new NoDefaultConstructorCommand("required parameter");
      }

      public object CreateModel(Type modelType)
      {
        return Activator.CreateInstance(modelType);
      }
    }
}