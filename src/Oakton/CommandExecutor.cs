using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JasperFx.Core;
using Oakton.Parsing;
using Spectre.Console;

namespace Oakton;

#nullable disable annotations // FIXME

/// <summary>
///     The main entry class for Oakton command line applications
/// </summary>
public class CommandExecutor
{
    public CommandExecutor(ICommandFactory factory)
    {
        Factory = factory;
    }

    public CommandExecutor() : this(new CommandFactory())
    {
    }

    /// <summary>
    ///     Directs Oakton to look for an options file at this location.
    /// </summary>
    public string OptionsFile { get; set; }


    public ICommandFactory Factory { get; }

    private static async Task<int> execute(CommandRun run)
    {
        bool success;

        try
        {
            success = await run.Execute();
        }
        catch (CommandFailureException e)
        {
            AnsiConsole.MarkupLine("[red]ERROR:[/]");
            AnsiConsole.WriteException(e);

            return 1;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]ERROR:[/]");
            AnsiConsole.WriteException(ex);

            return 1;
        }

        return success ? 0 : 1;
    }

    /// <summary>
    ///     Execute an instance of the "T" command class with the current arguments and an optional
    ///     opts file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="optsFile">
    ///     If chosen, Oakton will look at this file location for options and apply any found to the command
    ///     line arguments
    /// </param>
    /// <returns></returns>
    public static int ExecuteCommand<T>(string[] args, string optsFile = null) where T : IOaktonCommand
    {
        var factory = new CommandFactory();
        factory.RegisterCommand<T>();

        var executor = new CommandExecutor(factory)
        {
            OptionsFile = optsFile
        };

        return executor.Execute(args);
    }

    /// <summary>
    ///     Execute an instance of the "T" command class with the current arguments and an optional
    ///     opts file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="optsFile">
    ///     If chosen, Oakton will look at this file location for options and apply any found to the command
    ///     line arguments
    /// </param>
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
    ///     Build a configured executor. You would generally choose this option if you have multiple commands
    ///     within the application
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

    private string applyOptions(string commandLine)
    {
        if (OptionsFile.IsEmpty())
        {
            return commandLine;
        }

#if NET451
            var path = AppDomain.CurrentDomain.BaseDirectory.AppendPath(OptionsFile);
#else
        var path = AppContext.BaseDirectory.AppendPath(OptionsFile);
#endif

        if (File.Exists(path))
        {
            return $"{OptionReader.Read(path)} {commandLine}";
        }

        return commandLine;
    }

    internal static IEnumerable<string> ReadOptions(string optionsFile)
    {
        if (optionsFile.IsEmpty())
        {
            return new string[0];
        }

        var path = AppContext.BaseDirectory.AppendPath(optionsFile);

        if (File.Exists(path))
        {
            Console.WriteLine($"Found options in {optionsFile}");
            var options = OptionReader.Read(path);

            var values = StringTokenizer.Tokenize(options);
            return values;
        }

        Console.WriteLine($"Did not find expected options file at {path}");

        return new string[0];
    }

    /// <summary>
    ///     Execute with the command line arguments. Useful for testing Oakton applications
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    public int Execute(string commandLine)
    {
        return ExecuteAsync(commandLine).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Execute with the command line arguments.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public int Execute(string[] args)
    {
        return ExecuteAsync(args).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Execute asynchronously with the command line arguments. Useful for testing Oakton applications
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    public Task<int> ExecuteAsync(string commandLine)
    {
        commandLine = applyOptions(commandLine);
        var run = Factory.BuildRun(commandLine);

        return execute(run);
    }

    /// <summary>
    ///     Execute asynchronously with the command line arguments.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task<int> ExecuteAsync(string[] args)
    {
        var run = Factory.BuildRun(ReadOptions(OptionsFile).Concat(args));

        return execute(run);
    }
}