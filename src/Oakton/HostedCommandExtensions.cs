#nullable enable

using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Oakton.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Oakton;

public static class HostedCommandExtensions
{
    /// <summary>
    /// Register Oakton commands and services with the application's service collection.
    /// </summary>
    /// <param name="services"></param>
    public static void AddOakton(this IServiceCollection services, Action<OaktonOptions>? options = null)
    {
        services.Configure(options);

        services.TryAddSingleton<ICommandCreator, DependencyInjectionCommandCreator>();

        services.TryAddSingleton<ICommandFactory>((ctx) =>
        {
            var creator = ctx.GetRequiredService<ICommandCreator>();
            var oaktonOptions = ctx.GetRequiredService<IOptions<OaktonOptions>>().Value;

            var factory = new CommandFactory(creator);
            factory.ApplyFactoryDefaults(Assembly.GetEntryAssembly());
            oaktonOptions.Factory?.Invoke(factory);
            return factory;
        });

        services.TryAddSingleton<CommandExecutor>();
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured IHost.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application. This usage is appropriate for WebApplication bootstrapping
    /// </summary>
    /// <param name="host">An already built IHost</param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static int RunHostedOaktonCommands(this IHost host, string[] args)
    {
        return RunHostedOaktonCommandsAsync(host, args).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Execute the extended Oakton command line support for your configured IHost.
    ///     This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
    ///     of your AspNetCore application. This usage is appropriate for WebApplication bootstrapping.
    /// </summary>
    /// <param name="host">An already built IHost</param>
    /// <param name="args"></param>
    /// <param name="builder">Optionally configure additional command options</param>
    /// <returns></returns>
    public static Task<int> RunHostedOaktonCommandsAsync(this IHost host, string[] args)
    {
        var options = host.Services.GetRequiredService<IOptions<OaktonOptions>>().Value;
        args = ApplyArgumentDefaults(args, options);

        var executor = host.Services.GetRequiredService<CommandExecutor>();

        if (executor.Factory is CommandFactory factory)
        {
            var originalConfigureRun = factory.ConfigureRun;
            factory.ConfigureRun = cmd =>
            {
                if (cmd.Input is IHostBuilderInput i)
                {
                    i.HostBuilder = new PreBuiltHostBuilder(host);
                }

                originalConfigureRun?.Invoke(cmd);
            };
        }

        return executor.ExecuteAsync(args);
    }

    private static string[] ApplyArgumentDefaults(string[] args, OaktonOptions options)
    {
        // Workaround for IISExpress / VS2019 erroneously putting crap arguments
        args = args.FilterLauncherArgs();

        // Gotta apply the options file here before the magic "run" gets in
        if (options.OptionsFile.IsNotEmpty())
        {
            args = CommandExecutor.ReadOptions(options.OptionsFile).Concat(args).ToArray();
        }

        if (args == null || args.Length == 0 || args[0].StartsWith('-'))
        {
            args = new[] { options.DefaultCommand }.Concat(args ?? Array.Empty<string>()).ToArray();
        }

        return args;
    }
}
