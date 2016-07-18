using System;

namespace Oakton
{
    public class CommandExecutor
    {
        private readonly ICommandFactory _factory;

        public static int ExecuteInConsole<T>(string[] args) where T : CommandExecutor, new()
        {
            bool success;

            try
            {
                var executor = new T();
                success = executor.Execute(args);
            }
            catch (CommandFailureException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + e.Message);
                Console.ResetColor();
                return 1;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + ex);
                Console.ResetColor();
                return 1;
            }

            return success ? 0 : 1;
        }

        public CommandExecutor(ICommandFactory factory)
        {
            _factory = factory;
        }

        public CommandExecutor() : this(new CommandFactory())
        {
            
        }

        public ICommandFactory Factory => _factory;

        public bool Execute(string commandLine)
        {
            var run = _factory.BuildRun(commandLine);
            return run.Execute();
        }

        public bool Execute(string[] args)
        {
            var run = _factory.BuildRun(args);
            return run.Execute();
        }
    }
}