using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Oakton.Resources
{
    public static class ResourceHostExtensions
    {
        /// <summary>
        /// Add a hosted service that will do setup on all registered stateful resources
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddResourceSetupOnStartup(this IServiceCollection services)
        {
            if (!services.Any(x =>
                    x.ServiceType == typeof(IHostedService) &&
                    x.ImplementationType == typeof(ResourceSetupHostService)))
            {
                services.Insert(0, new ServiceDescriptor(typeof(IHostedService), typeof(ResourceSetupHostService), ServiceLifetime.Singleton));
                services.AddLogging();
            }

            return services;
        }
        
        /// <summary>
        /// Add a hosted service that will do setup on all registered stateful resources
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder UseResourceSetupOnStartup(this IHostBuilder builder)
        {
            return builder.ConfigureServices(s => s.AddResourceSetupOnStartup());
        }
        
        /// <summary>
        /// Add a hosted service that will do setup on all registered stateful resources, but only
        /// if the environment name is "Development"
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IHostBuilder UseResourceSetupOnStartupInDevelopment(this IHostBuilder builder, params string[] types)
        {
            return builder.ConfigureServices((context, services) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    services.AddResourceSetupOnStartup();
                }
            });
        }

        /// <summary>
        /// Executes SetUp(), then ClearState() on all stateful resources. Useful for automated testing scenarios to
        /// ensure all resources are in a good, known state
        /// </summary>
        /// <param name="host"></param>
        /// <param name="cancellation"></param>
        /// <param name="resourceType">Optional filter on resource type name</param>
        /// <param name="resourceName">Optional filter on resource name</param>
        public static async Task ResetResourceState(this IHost host, CancellationToken cancellation = default, string resourceType = null, string resourceName = null)
        {
            var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
            foreach (var resource in resources)
            {
                await resource.Setup(cancellation);
                await resource.ClearState(cancellation);
            }
        }
        
        /// <summary>
        /// Executes SetUp() on all stateful resources. Useful for automated testing scenarios to
        /// ensure all resources are in a good, known state
        /// </summary>
        /// <param name="host"></param>
        /// <param name="cancellation"></param>
        /// <param name="resourceType">Optional filter on resource type name</param>
        /// <param name="resourceName">Optional filter on resource name</param>
        public static async Task SetupResources(this IHost host, CancellationToken cancellation = default, string resourceType = null, string resourceName = null)
        {
            var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
            foreach (var resource in resources)
            {
                await resource.Setup(cancellation);
            }
        }
        
        /// <summary>
        /// Executes Teardown() on all stateful resources
        /// </summary>
        /// <param name="host"></param>
        /// <param name="cancellation"></param>
        /// <param name="resourceType">Optional filter on resource type name</param>
        /// <param name="resourceName">Optional filter on resource name</param>
        public static async Task TeardownResources(this IHost host, CancellationToken cancellation = default, string resourceType = null, string resourceName = null)
        {
            var resources = ResourcesCommand.FindResources(host.Services, resourceType, resourceName);
            foreach (var resource in resources)
            {
                await resource.Teardown(cancellation);
            }
        }

    }
}