using System;

namespace Oakton
{
    public class CommandExecutor
    {
        private readonly ICommandFactory _factory;

        private static int execute(Func<bool> execute)
        {
            bool success;

            try
            {
                success = execute();
            }
            catch (CommandFailureException e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "ERROR: " + e.Message);

                return 1;
            }
            catch (Exception ex)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "ERROR: " + ex);

                return 1;
            }

            return success ? 0 : 1;
        }

        public static int ExecuteInConsole<T>(string[] args) where T : CommandExecutor, new()
        {
            return execute(() =>
            {
                var executor = new T();
                return executor.Execute(args) == 0; // hokey. 
            });
        }

        public static CommandExecutor For(Action<CommandFactory> configure, ICommandCreator creator = null)
        {
            var factory = new CommandFactory(creator ?? new ActivatorCommandCreator());

            configure(factory);

            return new CommandExecutor(factory);
        }

        public CommandExecutor(ICommandFactory factory)
        {
            _factory = factory;
        }

        public CommandExecutor() : this(new CommandFactory())
        {
            
        }



        public ICommandFactory Factory => _factory;

        public int Execute(string commandLine)
        {
            return execute(() =>
            {
                var run = _factory.BuildRun(commandLine);
                return run.Execute();
            });

        }

        public int Execute(string[] args)
        {
            return execute(() =>
            {
                var run = _factory.BuildRun(args);
                return run.Execute();
            });
        }
    }
}