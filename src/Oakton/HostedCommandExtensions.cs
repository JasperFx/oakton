#nullable enable

using JasperFx.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Oakton;

public static class HostedCommandExtensions
{
    /// <summary>
    /// Register Oakton commands and services with the application's service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="factoryBuilder"></param>
    public static void AddOakton(this IServiceCollection services, Action<CommandFactory>? factoryBuilder = null)
    {
        var registrationFactory = new CommandFactory();
        registrationFactory.ApplyFactoryDefaults(factoryBuilder);
        registrationFactory.ApplyExtensions(services);

        var commands = registrationFactory.AllCommandTypes();
        foreach (var commandType in commands)
        {
            if (commandType.IsConcrete() && commandType.CanBeCastTo<IOaktonCommand>())
            {
                services.AddScoped(commandType);
            }
        }

        services.TryAddSingleton<ICommandCreator, DependencyInjectionCommandCreator>();

        services.TryAddSingleton<ICommandFactory>((ctx) =>
        {
            var creator = ctx.GetRequiredService<ICommandCreator>();
            var factory = new CommandFactory(creator);
            factory.ApplyFactoryDefaults(factoryBuilder);
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
    /// <param name="builder">Optionally configure additional command options</param>
    /// <returns></returns>
    public static int RunHostedOaktonCommands(this IHost host, string[] args, Action<HostedCommandOptions>? builder = null)
    {
        return RunHostedOaktonCommandsAsync(host, args, builder).GetAwaiter().GetResult();
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
    public static Task<int> RunHostedOaktonCommandsAsync(this IHost host, string[] args, Action<HostedCommandOptions>? builder = null)
    {
        var options = new HostedCommandOptions();
        builder?.Invoke(options);

        args = args.ApplyArgumentDefaults(options.OptionsFile);

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

    private static void ApplyFactoryDefaults(this CommandFactory factory, Action<CommandFactory>? builder = null)
    {
        factory.ApplyFactoryDefaults(Assembly.GetEntryAssembly());
        builder?.Invoke(factory);
    }
}
