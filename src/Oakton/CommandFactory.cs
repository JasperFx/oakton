using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Baseline;
using Oakton.Help;
using Oakton.Parsing;

namespace Oakton
{
    public class CommandFactory : ICommandFactory
    {
        private static readonly string[] _helpCommands = new []{"help", "?"}; 
        private readonly LightweightCache<string, Type> _commandTypes = new LightweightCache<string, Type>();
        private readonly ICommandCreator _commandCreator;
        private string _appName;

        public CommandFactory()
        {
            _commandCreator = new ActivatorCommandCreator();
        }

        public CommandFactory(ICommandCreator creator)
        {
            _commandCreator = creator;
        }

        /// <summary>
        /// Alter the input object or the command object just befor executing the command
        /// </summary>
        public Action<CommandRun> ConfigureRun = run => { };

        public CommandRun BuildRun(string commandLine)
        {
            var args = StringTokenizer.Tokenize(commandLine);
            return BuildRun(args);
        }

        public CommandRun BuildRun(IEnumerable<string> args)
        {

            if (!args.Any())
            {
                if (DefaultCommand == null)
                {
                    return HelpRun(new Queue<string>());
                }
            }

            args = ArgPreprocessor.Process(args);

            var queue = new Queue<string>(args);

            if (queue.Count == 0 && DefaultCommand != null)
            {
                return buildRun(queue, CommandNameFor(DefaultCommand));
            }

            var commandName = queue.Peek().ToLowerInvariant();

            if (commandName == "dump-usages")
            {
                queue.Dequeue();
                return dumpUsagesRun(queue);
            }

            if (_helpCommands.Contains(commandName))
            {
                queue.Dequeue();
                return HelpRun(queue);
            }

            if (_commandTypes.Has(commandName))
            {
                queue.Dequeue();
                return buildRun(queue, commandName);
            }
            if (DefaultCommand != null)
            {
                return buildRun(queue, CommandNameFor(DefaultCommand));
            }
            else
            {
                return InvalidCommandRun(commandName);
            }
        }


        public IEnumerable<Type> AllCommandTypes()
        {
            return _commandTypes.GetAll();
        }

        public CommandRun InvalidCommandRun(string commandName)
        {
            return new CommandRun()
            {
                Command = new HelpCommand(),
                Input = new HelpInput(){
                    AppName = _appName,
                    Name = commandName,
                    CommandTypes = _commandTypes.GetAll(),
                    InvalidCommandName = true
                }
            };
        }

        private CommandRun buildRun(Queue<string> queue, string commandName)
        {
            var command = Build(commandName);

            // this is where we'll call into UsageGraph?
            try
            {

                var usageGraph = command.Usages;
                var input = usageGraph.BuildInput(queue);

                var run = new CommandRun
                       {
                           Command = command,
                           Input = input
                       };

                ConfigureRun(run);

                return run;
            }
            catch (InvalidUsageException e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Invalid usage");


                if (e.Message.IsNotEmpty())
                {
                    ConsoleWriter.Write(ConsoleColor.Yellow, e.Message);
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Error parsing input");
                ConsoleWriter.Write(ConsoleColor.Yellow, e.ToString());
                
                Console.WriteLine();
            }

            return HelpRun(commandName);
        }

        /// <summary>
        /// Add a single command type to the command runner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterCommand<T>()
        {
            _commandTypes[CommandNameFor(typeof(T))] = typeof(T);
        }

        /// <summary>
        /// Add all the IOaktonCommand classes in the given assembly to the command runner
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterCommands(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(x => x.Closes(typeof(OaktonCommand<>)) && x.IsConcrete())
                .Each(t => { _commandTypes[CommandNameFor(t)] = t; });
        }

        private Type _defaultCommand = null;

        /// <summary>
        /// Optionally designates the default command type. Useful if your console app only has one command
        /// </summary>
        public Type DefaultCommand
        {
            get { return _defaultCommand ?? (_commandTypes.Count == 1 ? _commandTypes.GetAll().Single() : null); }
            set
            {
                _defaultCommand = value;
                if (value != null) _commandTypes[CommandNameFor(value)] = value;
            }
        }

        public IEnumerable<IOaktonCommand> BuildAllCommands()
        {
            return _commandTypes.Select(x => _commandCreator.Create(x));
        }


        public IOaktonCommand Build(string commandName)
        {
            return _commandCreator.Create(_commandTypes[commandName.ToLower()]);
        }



        public CommandRun HelpRun(string commandName)
        {
            return HelpRun(new Queue<string>(new []{commandName}));
        }

        public virtual CommandRun HelpRun(Queue<string> queue)
        {
            var input = (HelpInput) (new HelpCommand().Usages.BuildInput(queue));
            input.CommandTypes = _commandTypes.GetAll();


            if (input.Name.IsNotEmpty())
            {
                input.InvalidCommandName = true;
                input.Name = input.Name.ToLowerInvariant();
                _commandTypes.WithValue(input.Name, type =>
                {
                    input.InvalidCommandName = false;
                    input.Usage = new UsageGraph(type);
                });
            }

            return new CommandRun(){
                Command = new HelpCommand(),
                Input = input
            };
        }

        private CommandRun dumpUsagesRun(Queue<string> queue)
        {
            var command = new DumpUsagesCommand();
            var input = command.Usages.BuildInput(queue).As<DumpUsagesInput>();
            input.Commands = this;
            
            return new CommandRun
            {
                Command = command,
                Input = input
            };
        }


        static readonly Regex regex = new Regex("(?<name>.+)Command",RegexOptions.Compiled);
        public static string CommandNameFor(Type type)
        {
            
            var match = regex.Match(type.Name);
            var name = type.Name;
            if(match.Success)
            {
                name = match.Groups["name"].Value;
            }
            
            type.ForAttribute<DescriptionAttribute>(att => name = att.Name ?? name);

            return name.ToLower();
        }

        public static string DescriptionFor(Type type)
        {
            var description = type.FullName;
            type.ForAttribute<DescriptionAttribute>(att => description = att.Description);

            return description;
        }

        public void SetAppName(string appName)
        {
            _appName = appName;
        }
    }
}