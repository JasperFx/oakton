using System;
using Xunit;

namespace Oakton.Testing
{
    
    public class CommandExecutorTester
    {
        private ICommandFactory factory;
        private IOaktonCommand command;
        private CommandExecutor theExecutor;
        private object theInput;
        private string commandLine;

        /*
        public CommandExecutorTester()
        {
            factory = MockRepository.GenerateMock<ICommandFactory>();
            command = MockRepository.GenerateMock<IOaktonCommand>();
            theInput = new object();
            commandLine = "some stuff here";

            theExecutor = new CommandExecutor(factory);
        }


        [Fact]
        public void run_command_happy_path_executes_the_command_with_the_input()
        {
            factory.Stub(x => x.BuildRun(commandLine)).Return(new CommandRun(){
                Command = command,
                Input = theInput
            });

            theExecutor.Execute(commandLine);

            command.AssertWasCalled(x => x.Execute(theInput));
        }
        */

        [Fact]
        public void redo()
        {
            throw new NotImplementedException();
        }
    }
}