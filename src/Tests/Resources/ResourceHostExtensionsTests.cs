using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Oakton.Resources;
using Shouldly;
using Xunit;

namespace Tests.Resources
{
    public class ResourceHostExtensionsTests : ResourceCommandContext
    {
        public static async Task sample1()
        {
            #region sample_using_AddResourceSetupOnStartup

            using var host = await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // More service registrations like this is a real app!

                    services.AddResourceSetupOnStartup();
                }).StartAsync();

            #endregion
        }
        
        public static async Task sample2()
        {
            #region sample_using_AddResourceSetupOnStartup2

            using var host = await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // More service registrations like this is a real app!
                })
                .UseResourceSetupOnStartup()
                .StartAsync();

            #endregion
        }
        
        public static async Task sample3()
        {
            #region sample_using_AddResourceSetupOnStartup3

            using var host = await Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // More service registrations like this is a real app!
                })
                .UseResourceSetupOnStartupInDevelopment()
                .StartAsync();

            #endregion
        }

        #region sample_programmatically_control_resources

        public static async Task usages_for_testing(IHost host)
        {
            // Programmatically call Setup() on all resources
            await host.SetupResources();
            
            // Maybe between integration tests, clear any
            // persisted state. For example, I've used this to 
            // purge Rabbit MQ queues between tests
            await host.ResetResourceState();

            // Tear it all down!
            await host.TeardownResources();
        }

        #endregion
        
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
                await resource.DidNotReceive().ClearState(Arg.Any<CancellationToken>());
            }

        }
        
        
        [Fact]
        public async Task runs_all_resources_and_resets()
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
                    services.AddResourceSetupOnStartup(StartupAction.ResetState);
                })
                .StartAsync();

            foreach (var resource in AllResources)
            {
                await resource.Received().Setup(Arg.Any<CancellationToken>());
                await resource.Received().ClearState(Arg.Any<CancellationToken>());
            }

        }

        [Fact]
        public async Task setup_all()
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

            using var host = await buildHost();
            await host.SetupResources();

            foreach (var resource in AllResources)
            {
                await resource.Received().Setup(Arg.Any<CancellationToken>());
            }
            
            
        }
        
        [Fact]
        public async Task reset_all()
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

            using var host = await buildHost();
            await host.ResetResourceState();

            foreach (var resource in AllResources)
            {
                await resource.Received().Setup(Arg.Any<CancellationToken>());
                await resource.Received().ClearState(Arg.Any<CancellationToken>());
            }
            
            
        }
        
        [Fact]
        public async Task teardown_all()
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

            using var host = await buildHost();
            await host.TeardownResources();

            foreach (var resource in AllResources)
            {
                await resource.Received().Teardown(Arg.Any<CancellationToken>());
            }
            
            
        }
    }
}