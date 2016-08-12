using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Baseline;
using Oakton.Parsing;

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

        public string OptionsFile { get; set; }


        public ICommandFactory Factory => _factory;

        private string applyOptions(string commandLine)
        {
            if (OptionsFile.IsEmpty()) return commandLine;

#if NET451
            var path = AppDomain.CurrentDomain.BaseDirectory.AppendPath(OptionsFile);
#else
            var path = AppContext.BaseDirectory.AppendPath(OptionsFile);
#endif

            if (File.Exists(path))
            {
                return $"{OptionReader.Read(path)} {commandLine}";
            }
            else
            {
                return commandLine;
            }
        }

        private IEnumerable<string> readOptions()
        {
            if (OptionsFile.IsEmpty()) return new string[0];

#if NET451
            var path = AppDomain.CurrentDomain.BaseDirectory.AppendPath(OptionsFile);
#else
            var path = AppContext.BaseDirectory.AppendPath(OptionsFile);
#endif

            if (File.Exists(path))
            {
                var options = OptionReader.Read(path);

                return StringTokenizer.Tokenize(options);
            }
            else
            {
                return new string[0];
            }
        }

        public int Execute(string commandLine)
        {
            commandLine = applyOptions(commandLine);

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
                var run = _factory.BuildRun(readOptions().Concat(args));
                return run.Execute();
            });
        }
    }
}