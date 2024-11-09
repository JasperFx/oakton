using JasperFx.Core;
using JasperFx.Core.Reflection;
using JasperFx.Core.TypeScanning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.Help;
using Oakton.Parsing;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Oakton;

#nullable disable annotations // FIXME

public class CommandFactory : ICommandFactory
{
    private static readonly string[] _helpCommands = { "help", "?" };


    private static readonly Regex regex = new("(?<name>.+)Command", RegexOptions.Compiled);

    // Only used to disable the console writing on errors
    private static bool _hasAppliedExtensions;
    private readonly ICommandCreator _commandCreator;
    private readonly LightweightCache<string, Type> _commandTypes = new();
    private string _appName;

    private Type _defaultCommand;

    private readonly IList<Type> _extensionTypes = new List<Type>();

    /// <summary>
    ///     Perform some operation based on command inputs, before command construction
    /// </summary>
    public Action<string, object> BeforeBuild = null;

    /// <summary>
    ///     Alter the input object or the command object just before executing the command
    /// </summary>
    public Action<CommandRun> ConfigureRun = run => { };

    public CommandFactory()
    {
        _commandCreator = new ActivatorCommandCreator();
    }

    public CommandFactory(ICommandCreator creator)
    {
        _commandCreator = creator;
    }

    /// <summary>
    ///     Optionally designates the default command type. Useful if your console app only has one command
    /// </summary>
    public Type DefaultCommand
    {
        get => _defaultCommand ?? (_commandTypes.Count == 1 ? _commandTypes.Single() : null);
        set
        {
            _defaultCommand = value;
            if (value != null)
            {
                _commandTypes[CommandNameFor(value)] = value;
            }
        }
    }

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

        if (_helpCommands.Contains(firstArg))
        {
            queue.Dequeue();

            return HelpRun(queue);
        }

        if (_commandTypes.Contains(firstArg))
        {
            queue.Dequeue();
            return buildRun(queue, firstArg);
        }

        if (DefaultCommand != null)
        {
            return buildRun(queue, CommandNameFor(DefaultCommand));
        }

        return InvalidCommandRun(firstArg);
    }

    /// <summary>
    ///     Add all the IOaktonCommand classes in the given assembly to the command runner
    /// </summary>
    /// <param name="assembly"></param>
    public void RegisterCommands(Assembly assembly)
    {
        foreach (var type in assembly
                     .GetExportedTypes()
                     .Where(IsOaktonCommandType))
            _commandTypes[CommandNameFor(type)] = type;

        if (assembly.HasAttribute<OaktonCommandAssemblyAttribute>())
        {
            var att = assembly.GetAttribute<OaktonCommandAssemblyAttribute>();
            if (att.ExtensionType != null)
            {
                _extensionTypes.Add(att.ExtensionType);
            }
        }
    }

    public IEnumerable<IOaktonCommand> BuildAllCommands()
    {
        return _commandTypes.Select(x => _commandCreator.CreateCommand(x));
    }

    public void ApplyExtensions(IServiceCollection services)
    {
        try
        {
            foreach (var extensionType in _extensionTypes)
            {
                var extension = Activator.CreateInstance(extensionType) as IServiceRegistrations;
                extension?.Configure(services);
            }

            _hasAppliedExtensions = true;
        }
        catch (Exception)
        {
            // Swallow the error
            if (_hasAppliedExtensions)
            {
                return;
            }

            AnsiConsole.MarkupLine(
                $"[red]Unable to apply Oakton extensions. Try adding IHostBuilder.{nameof(CommandLineHostingExtensions.ApplyOaktonExtensions)}(); to your bootstrapping code to apply Oakton extension loading[/]");
        }
    }

    public void ApplyExtensions(IHostBuilder builder)
    {
        if (builder is PreBuiltHostBuilder) return;
        
        if (_extensionTypes.Any())
        {
            builder.ConfigureServices(ApplyExtensions);
        }
    }

    public IEnumerable<Type> AllCommandTypes()
    {
        return _commandTypes;
    }

    public CommandRun InvalidCommandRun(string commandName)
    {
        return new CommandRun
        {
            Command = new HelpCommand(),
            Input = new HelpInput
            {
                AppName = _appName,
                Name = commandName,
                CommandTypes = _commandTypes.ToArray(),
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

            input ??= command.Usages.BuildInput(queue, _commandCreator);
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
            AnsiConsole.MarkupLine("[red]Invalid usage[/]");

            if (e.Message.IsNotEmpty())
            {
                AnsiConsole.MarkupLine($"[yellow]{e.Message.EscapeMarkup()}[/]");
            }

            Console.WriteLine();
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]Error parsing input[/]");
            AnsiConsole.WriteException(e);

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
    ///     Add a single command type to the command runner
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterCommand<T>()
    {
        RegisterCommand(typeof(T));
    }

    /// <summary>
    ///     Add a single command type to the command runner
    /// </summary>
    public void RegisterCommand(Type type)
    {
        if (!IsOaktonCommandType(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type),
                $"Type '{type.FullName}' does not inherit from either OaktonCommannd or OaktonAsyncCommand");
        }

        _commandTypes[CommandNameFor(type)] = type;
    }

    public static bool IsOaktonCommandType(Type type)
    {
        if (!type.IsConcrete())
        {
            return false;
        }

        return type.Closes(typeof(OaktonCommand<>)) || type.Closes(typeof(OaktonAsyncCommand<>));
    }


    public IOaktonCommand Build(string commandName)
    {
        return _commandCreator.CreateCommand(_commandTypes[commandName.ToLower()]);
    }


    public CommandRun HelpRun(string commandName)
    {
        return HelpRun(new Queue<string>(new[] { commandName }));
    }

    public virtual CommandRun HelpRun(Queue<string> queue)
    {
        var input = (HelpInput)new HelpCommand().Usages.BuildInput(queue, _commandCreator);
        input.CommandTypes = _commandTypes.ToArray();

        // Little hokey, but show the detailed help for the default command
        if (DefaultCommand != null && input.CommandTypes.Count() == 1)
        {
            input.Name = CommandNameFor(DefaultCommand);
        }


        if (input.Name.IsNotEmpty())
        {
            input.InvalidCommandName = true;
            input.Name = input.Name.ToLowerInvariant();

            if (_commandTypes.TryFind(input.Name, out var type))
            {
                input.InvalidCommandName = false;

                var cmd = _commandCreator.CreateCommand(type);

                input.Usage = cmd.Usages;
            }
        }

        return new CommandRun
        {
            Command = new HelpCommand(),
            Input = input
        };
    }

    public static string CommandNameFor(Type type)
    {
        var match = regex.Match(type.Name);
        var name = type.Name;
        if (match.Success)
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
    ///     Automatically discover any Oakton commands in assemblies marked as
    ///     [assembly: OaktonCommandAssembly]. Also
    /// </summary>
    public void RegisterCommandsFromExtensionAssemblies()
    {
        var assemblies = AssemblyFinder
            .FindAssemblies(a => a.HasAttribute<OaktonCommandAssemblyAttribute>() && !a.IsDynamic)
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .Where(a => a.HasAttribute<OaktonCommandAssemblyAttribute>() && !a.IsDynamic)
            .Distinct()
            .ToArray();

        foreach (var assembly in assemblies)
        {
            if (!_hasAppliedExtensions)
            {
                AnsiConsole.MarkupLine($"[gray]Searching '{assembly.FullName}' for commands[/]");
            }

            RegisterCommands(assembly);
        }

        AnsiConsole.WriteLine();
    }
}