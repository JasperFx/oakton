using System;
using Oakton;
using Shouldly;
using Xunit;

namespace Tests
{
    public class CustomCommandCreatorTests
    {
        [Fact]
        public void command_factory_creates_with_custom_creator()
        {
            TestCommandCreator creator = new TestCommandCreator();
            ICommandFactory factory = new CommandFactory(creator);
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommand<TestCommand>();
            }, creator);

            executor.Execute("test");

            creator.LastCreatedModel.ShouldNotBeNull();
        }

        [Fact]
        public void help_displays_with_custom_creator()
        {
            TestCommandCreator creator = new TestCommandCreator();
            CommandFactory factory = new CommandFactory(creator);
            var executor = CommandExecutor.For(_ =>
            {
                _.RegisterCommand<TestCommand>();
            }, creator);

            executor.Execute("");

            creator.LastCreatedModel.ShouldNotBeNull();
        }

        private class TestCommand : OaktonCommand<object>
        {
            public TestCommand(string message)
            {
                Message = message;
            }

            public string Message { get; }

            public override bool Execute(object input)
            {
                return true;
            }
        }

        private class TestOptions
        {
            public TestOptions(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        private class TestCommandCreator : ICommandCreator
        {
            public TestCommand LastCreatedCommand { get; private set; }

            public TestOptions LastCreatedModel { get; private set; }

            public IOaktonCommand CreateCommand(Type commandType)
            {
                LastCreatedCommand = new TestCommand("created command");
                return LastCreatedCommand;
            }

            public object CreateModel(Type modelType)
            {
                LastCreatedModel = new TestOptions("created options");
                return LastCreatedModel;
            }
        }
    }
}
