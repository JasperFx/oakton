using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Oakton.Resources;
using Shouldly;
using Xunit;

namespace Tests.Resources
{
    public class ResourceHostExtensionsTests : ResourceCommandContext
    {
        [Fact]
        public void add_resource_startup()
        {
            using var container = Container.For(services =>
            {
                services.AddResourceSetupOnStartup();
                
                // Only does it once!
                services.AddResourceSetupOnStartup();
                services.AddResourceSetupOnStartup();
                services.AddResourceSetupOnStartup();
                services.AddResourceSetupOnStartup();
            });
            
            container.Model.For<IHostedService>()
                .Instances.Single().ImplementationType.ShouldBe(typeof(ResourceSetupHostService));

            container.GetInstance<IHostedService>()
                .ShouldBeOfType<ResourceSetupHostService>();
        }

        [Fact]
        public void use_resource_setup()
        {
            using var host = Host.CreateDefaultBuilder()
                .UseLamar()
                .UseResourceSetupOnStartup()
                .Build();

            var container = (IContainer)host.Services;
            
            container.Model.For<IHostedService>()
                .Instances.Single().ImplementationType.ShouldBe(typeof(ResourceSetupHostService));

            container.GetInstance<IHostedService>()
                .ShouldBeOfType<ResourceSetupHostService>();
        }

        [Fact]
        public void use_conditional_resource_setup_in_development()
        {
            using var host = Host.CreateDefaultBuilder()
                .UseLamar()
                .UseResourceSetupOnStartupInDevelopment()
                .UseEnvironment("Development")
                .Build();

            var container = (IContainer)host.Services;
            
            container.Model.For<IHostedService>()
                .Instances.Single().ImplementationType.ShouldBe(typeof(ResourceSetupHostService));

            container.GetInstance<IHostedService>()
                .ShouldBeOfType<ResourceSetupHostService>();
        }
        
        [Fact]
        public void use_conditional_resource_setup_only_in_development_does_nothing_in_prod()
        {
            using var host = Host.CreateDefaultBuilder()
                .UseLamar()
                .UseResourceSetupOnStartupInDevelopment()
                .UseEnvironment("Production")
                .Build();

            var container = (IContainer)host.Services;
            
            container.Model.For<IHostedService>()
                .Instances.Any().ShouldBeFalse();
        }

        [Fact]
        public async Task runs_all_resources()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");

            AddSource(col =>
            {
                col.Add("purple", "color");
                col.Add("orange", "color");
            });
            
            AddSource(col =>
            {
                col.Add("green", "color");
                col.Add("white", "color");
            });

            using var host = await Host.CreateDefaultBuilder()
                .UseResourceSetupOnStartup()
                .ConfigureServices(services =>
                {
                    CopyResources(services);
                    services.AddResourceSetupOnStartup();
                })
                .StartAsync();

            foreach (var resource in AllResources)
            {
                await resource.Received().Setup(Arg.Any<CancellationToken>());
            }

        }
    }
}