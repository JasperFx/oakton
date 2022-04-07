using System.Linq;
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
    }
}