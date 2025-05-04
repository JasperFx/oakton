using JasperFx.Core;
using Microsoft.Extensions.Hosting;
using Oakton.Commands;
using Oakton.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JasperFx.Core.Reflection;

namespace Oakton;

public static class CommandLineHostingExtensions
{
    /// <summary>
    ///     Discover and apply Oakton extensions to this application during
    ///     bootstrapping. This is only necessary when using the WebApplication
    ///     approach to bootstrapping applications introduced in .Net 6
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostBuilder ApplyOaktonExtensions(this IHostBuilder builder)
    {
        var factory = new CommandFactory();
        factory.RegisterCommandsFromExtensionAssemblies();
        factory.ApplyExtensions(builder);

        return builder;
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured WebHostBuilder.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="optionsFile">Optionally configure an expected "opts" file</param>
    /// <returns></returns>
    public static Task<int> RunOaktonCommands(this IHostBuilder builder, string[] args, string? optionsFile = null)
    {
        return execute(builder, Assembly.GetEntryAssembly(), args, optionsFile);
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured WebHostBuilder.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="args"></param>
    /// <param name="optionsFile">Optionally configure an expected "opts" file</param>
    /// <returns></returns>
    public static int RunOaktonCommandsSynchronously(this IHostBuilder builder, string[] args,
        string? optionsFile = null)
    {
        return execute(builder, Assembly.GetEntryAssembly(), args, optionsFile).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured IHost.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application. This usage is appropriate for WebApplication bootstrapping
    /// </summary>
    /// <param name="host">An already built IHost</param>
    /// <param name="args"></param>
    /// <param name="optionsFile">Optionally configure an expected "opts" file</param>
    /// <returns></returns>
    public static Task<int> RunOaktonCommands(this IHost host, string[] args, string? optionsFile = null)
    {
        return execute(new PreBuiltHostBuilder(host), Assembly.GetEntryAssembly(), args, optionsFile);
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured IHost.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application. This usage is appropriate for WebApplication bootstrapping
    /// </summary>
    /// <param name="host">An already built IHost</param>
    /// <param name="args"></param>
    /// <param name="optionsFile">Optionally configure an expected "opts" file</param>
    /// <returns></returns>
    public static int RunOaktonCommandsSynchronously(this IHost host, string[] args, string? optionsFile = null)
    {
        return execute(new PreBuiltHostBuilder(host), Assembly.GetEntryAssembly(), args, optionsFile).GetAwaiter()
            .GetResult();
    }

    internal static string[] ApplyArgumentDefaults(this string[] args, string? optionsFile)
    {
        // Workaround for IISExpress / VS2019 erroneously putting crap arguments
        args = args.FilterLauncherArgs();

        // Gotta apply the options file here before the magic "run" gets in
        if (optionsFile.IsNotEmpty())
        {
            args = CommandExecutor.ReadOptions(optionsFile).Concat(args).ToArray();
        }

        if (args == null || args.Length == 0 || args[0].StartsWith('-'))
        {
            args = new[] { "run" }.Concat(args ?? Array.Empty<string>()).ToArray();
        }

        return args;
    }

    internal static void ApplyFactoryDefaults(this CommandFactory factory, Assembly? applicationAssembly)
    {
        factory.RegisterCommands(typeof(RunCommand).GetTypeInfo().Assembly);

        if (applicationAssembly != null)
        {
            factory.RegisterCommands(applicationAssembly);
        }

        factory.RegisterCommandsFromExtensionAssemblies();
    }

    private static Task<int> execute(IHostBuilder runtimeSource, Assembly? applicationAssembly, string[] args,
        string? optionsFile)
    {
        args = args.ApplyArgumentDefaults(optionsFile);

        var commandExecutor = buildExecutor(runtimeSource, applicationAssembly);
        return commandExecutor.ExecuteAsync(args);
    }

    private static CommandExecutor buildExecutor(IHostBuilder source, Assembly? applicationAssembly)
    {
        if (OaktonEnvironment.AutoStartHost && source is PreBuiltHostBuilder b)
        {
            b.Host.Start();
        }

        #region sample_using_extension_assemblies

        return CommandExecutor.For(factory =>
        {
            factory.ApplyFactoryDefaults(applicationAssembly);

            factory.ConfigureRun = commandRun =>
            {
                if (commandRun.Input is IHostBuilderInput i)
                {
                    factory.ApplyExtensions(source);
                    i.HostBuilder = source;
                }
                else
                {
                    var props = commandRun.Command.GetType().GetProperties().Where(x => x.HasAttribute<InjectServiceAttribute>())
                        .ToArray();

                    if (props.Any())
                    {
                        commandRun.Command = new HostWrapperCommand(commandRun.Command, source.Build, props);
                    }
                }
                
                
            };
        });

        #endregion
    }
}