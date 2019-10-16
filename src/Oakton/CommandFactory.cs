using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Baseline;
using Baseline.Reflection;
using BaselineTypeDiscovery;
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
        /// Perform some operation based on command inputs, before command construction
        /// </summary>
        public Action<string, object> BeforeBuild = null;

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
            
            

            var firstArg = queue.Peek().ToLowerInvariant();

            if (firstArg == "dump-usages")
            {
                queue.Dequeue();
                return dumpUsagesRun(queue);
            }

            if (_helpCommands.Contains(firstArg))
            {
                queue.Dequeue();

                return HelpRun(queue);
            }

            if (_commandTypes.Has(firstArg))
            {
                queue.Dequeue();
                return buildRun(queue, firstArg);
            }
            
            if (DefaultCommand != null)
            {
                return buildRun(queue, CommandNameFor(DefaultCommand));
            }
            else
            {
                return InvalidCommandRun(firstArg);
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
            try
            {
                object input = null;

                if (BeforeBuild != null)
                {
                  input = tryBeforeBuild(queue, commandName);
                }

                var command = Build(commandName);

                if (input == null)
                {
                  input = command.Usages.BuildInput(queue, _commandCreator);
                }

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

        private object tryBeforeBuild(Queue<string> queue, string commandName)
        {
          var commandType = _commandTypes[commandName];

          try
          {
            var defaultConstructorCommand = new ActivatorCommandCreator().CreateCommand(commandType);
            var input = defaultConstructorCommand.Usages.BuildInput(queue, _commandCreator);

            BeforeBuild?.Invoke(commandName, input);

            return input;
          }
          catch (MissingMethodException)
          {
            // Command has no default constructor - not possible to pre-configure from inputs.
            return null;
          }
        }

        /// <summary>
        /// Add a single command type to the command runner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterCommand<T>()
        {
            RegisterCommand(typeof(T));
        }
        
        /// <summary>
        /// Add a single command type to the command runner
        /// </summary>
        public void RegisterCommand(Type type)
        {
            if (!IsOaktonCommandType(type)) throw new ArgumentOutOfRangeException(nameof(type), $"Type '{type.FullName}' does not inherit from either OaktonCommannd or OaktonAsyncCommand");
            _commandTypes[CommandNameFor(type)] = type;
        }

        /// <summary>
        /// Add all the IOaktonCommand classes in the given assembly to the command runner
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterCommands(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(IsOaktonCommandType)
                .Each(t => { _commandTypes[CommandNameFor(t)] = t; });
        }

        public static bool IsOaktonCommandType(Type type)
        {
            if (!type.IsConcrete()) return false;

            return type.Closes(typeof(OaktonCommand<>)) || type.Closes(typeof(OaktonAsyncCommand<>));
        }

        private Type _defaultCommand = null;

        /// <summary>
        /// Optionally designates the default command type. Useful if your console app only has one command
        /// </summary>
        public Type DefaultCommand
        {
            get => _defaultCommand ?? (_commandTypes.Count == 1 ? _commandTypes.GetAll().Single() : null);
            set
            {
                _defaultCommand = value;
                if (value != null) _commandTypes[CommandNameFor(value)] = value;
            }
        }

        public IEnumerable<IOaktonCommand> BuildAllCommands()
        {
            return _commandTypes.Select(x => _commandCreator.CreateCommand(x));
        }


        public IOaktonCommand Build(string commandName)
        {
            return _commandCreator.CreateCommand(_commandTypes[commandName.ToLower()]);
        }



        public CommandRun HelpRun(string commandName)
        {
            return HelpRun(new Queue<string>(new []{commandName}));
        }

        public virtual CommandRun HelpRun(Queue<string> queue)
        {
            var input = (HelpInput) (new HelpCommand().Usages.BuildInput(queue, _commandCreator));
            input.CommandTypes = _commandTypes.GetAll();

            // Little hokey, but show the detailed help for the default command
            if (DefaultCommand != null && input.CommandTypes.Count() == 1)
            {
                input.Name = CommandNameFor(DefaultCommand);
            }


            if (input.Name.IsNotEmpty())
            {
                input.InvalidCommandName = true;
                input.Name = input.Name.ToLowerInvariant();
                _commandTypes.WithValue(input.Name, type =>
                {
                    input.InvalidCommandName = false;

                    var cmd = _commandCreator.CreateCommand(type);
                    
                    input.Usage = cmd.Usages;
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
            var input = command.Usages.BuildInput(queue, _commandCreator).As<DumpUsagesInput>();
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
        
        /// <summary>
        /// Automatically discover any Oakton commands in assemblies marked as
        /// [assembly: OaktonCommandAssembly]. Also 
        /// </summary>
        /// <param name="applicationAssembly"></param>
        public void RegisterCommandsFromExtensionAssemblies()
        {
            var assemblies = AssemblyFinder
                .FindAssemblies(txt => { }, false)
                .Concat(AppDomain.CurrentDomain.GetAssemblies())
                .Distinct()
                .Where(a => a.HasAttribute<OaktonCommandAssemblyAttribute>())
                .ToArray();

            foreach (var assembly in assemblies)
            {
                Console.WriteLine($"Searching '{assembly.FullName}' for commands");
                RegisterCommands(assembly);
            }
        }
    }
}