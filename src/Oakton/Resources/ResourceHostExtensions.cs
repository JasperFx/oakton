using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

public enum StartupAction
{
    /// <summary>
    ///     Only check that each resource is set up and functional
    /// </summary>
    SetupOnly,

    /// <summary>
    ///     Check that each resource is set up, functional, and clear off
    ///     any existing state. This is mainly meant for automated testing scenarios
    /// </summary>
    ResetState
}

public static class ResourceHostExtensions
{
    /// <summary>
    ///     Add a hosted service that will do setup on all registered stateful resources
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action">Configure the startup action. The default is SetupOnly</param>
    /// <returns></returns>
    public static IServiceCollection AddResourceSetupOnStartup(this IServiceCollection services,
        StartupAction action = StartupAction.SetupOnly)
    {
        if (!services.Any(x =>
                x.ServiceType == typeof(IHostedService) &&
                x.ImplementationType == typeof(ResourceSetupHostService)))
        {
            services.Insert(0,
                new ServiceDescriptor(typeof(IHostedService), typeof(ResourceSetupHostService),
                    ServiceLifetime.Singleton));
            services.AddLogging();
        }

        var options = new ResourceSetupOptions { Action = action };
        services.AddSingleton(options);

        return services;
    }

    /// <summary>
    ///     Add a hosted service that will do setup on all registered stateful resources
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action">Configure the startup action. The default is SetupOnly</param>
    /// <returns></returns>
    public static IHostBuilder UseResourceSetupOnStartup(this IHostBuilder builder,
        StartupAction action = StartupAction.SetupOnly)
    {
        return builder.ConfigureServices(s => s.AddResourceSetupOnStartup(action));
    }

    /// <summary>
    ///     Add a hosted service that will do setup on all registered stateful resources, but only
    ///     if the environment name is "Development"
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action">Configure the startup action. The default is SetupOnly</param>
    /// <returns></returns>
    public static IHostBuilder UseResourceSetupOnStartupInDevelopment(this IHostBuilder builder,
        StartupAction action = StartupAction.SetupOnly)
    {
        return builder.ConfigureServices((context, services) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddResourceSetupOnStartup(action);
            }
        });
    }

    /// <summary>
    ///     Executes SetUp(), then ClearState() on all stateful resources. Useful for automated testing scenarios to
    ///     ensure all resources are in a good, known state
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellation"></param>
    /// <param name="resourceType">Optional filter on resource type name</param>
    /// <param name="resourceName">Optional filter on resource name</param>
    public static async Task ResetResourceState(this IHost host, CancellationToken cancellation = default,
        string resourceType = null, string resourceName = null)
    {
        var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
        foreach (var resource in resources)
        {
            await resource.Setup(cancellation);
            await resource.ClearState(cancellation);
        }
    }

    /// <summary>
    ///     Executes SetUp() on all stateful resources. Useful for automated testing scenarios to
    ///     ensure all resources are in a good, known state
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellation"></param>
    /// <param name="resourceType">Optional filter on resource type name</param>
    /// <param name="resourceName">Optional filter on resource name</param>
    public static async Task SetupResources(this IHost host, CancellationToken cancellation = default,
        string resourceType = null, string resourceName = null)
    {
        var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
        foreach (var resource in resources) await resource.Setup(cancellation);
    }

    /// <summary>
    ///     Executes Teardown() on all stateful resources
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellation"></param>
    /// <param name="resourceType">Optional filter on resource type name</param>
    /// <param name="resourceName">Optional filter on resource name</param>
    public static async Task TeardownResources(this IHost host, CancellationToken cancellation = default,
        string resourceType = null, string resourceName = null)
    {
        var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
        foreach (var resource in resources) await resource.Teardown(cancellation);
    }
}