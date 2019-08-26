using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Baseline;
using Baseline.Reflection;
using Oakton.Discovery;
using Oakton.Parsing;

namespace Oakton
{
    /// <summary>
    /// The main entry class for Oakton command line applications
    /// </summary>
    public class CommandExecutor
    {
        private readonly ICommandFactory _factory;

        private static async Task<int> execute(CommandRun run)
        {
            bool success;

            try
            {
                success = await run.Execute();
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

        /// <summary>
        /// Execute an instance of the "T" command class with the current arguments and an optional
        /// opts file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="optsFile">If chosen, Oakton will look at this file location for options and apply any found to the command line arguments</param>
        /// <returns></returns>
        public static int ExecuteCommand<T>(string[] args, string optsFile = null) where T : IOaktonCommand
        {
            var factory = new CommandFactory();
            factory.RegisterCommand<T>();

            var executor =  new CommandExecutor(factory)
            {
                OptionsFile = optsFile
            };

            return executor.Execute(args);
        }

        /// <summary>
        /// Execute an instance of the "T" command class with the current arguments and an optional
        /// opts file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="optsFile">If chosen, Oakton will look at this file location for options and apply any found to the command line arguments</param>
        /// <returns></returns>
        public static Task<int> ExecuteCommandAsync<T>(string[] args, string optsFile = null) where T : IOaktonCommand
        {
            var factory = new CommandFactory();
            factory.RegisterCommand<T>();

            var executor = new CommandExecutor(factory)
            {
                OptionsFile = optsFile
            };

            return executor.ExecuteAsync(args);
        }

        /// <summary>
        /// Build a configured executor. You would generally choose this option if you have multiple commands
        /// within the application
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Directs Oakton to look for an options file at this location. 
        /// </summary>
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

        /// <summary>
        /// Execute with the command line arguments. Useful for testing Oakton applications
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public int Execute(string commandLine)
        {
            return ExecuteAsync(commandLine).GetAwaiter().GetResult();

        }

        /// <summary>
        /// Execute with the command line arguments. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public int Execute(string[] args)
        {
            return ExecuteAsync(args).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute asynchronously with the command line arguments. Useful for testing Oakton applications
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string commandLine)
        {
            commandLine = applyOptions(commandLine);
            var run = _factory.BuildRun(commandLine);

            return execute(run);

        }

        /// <summary>
        /// Execute asynchronously with the command line arguments. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string[] args)
        {
            var run = _factory.BuildRun(readOptions().Concat(args));

            return execute(run);
        }



    }
}