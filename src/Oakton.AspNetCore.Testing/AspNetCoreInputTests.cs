using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
#if NETCOREAPP2_2
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
#else
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using HostBuilder = Microsoft.Extensions.Hosting.HostBuilder;
using WebHostBuilder = Microsoft.AspNetCore.Hosting.WebHostBuilder;

namespace Oakton.AspNetCore.Testing
{
    public class AspNetCoreInputTests
    {
        [Fact]
        public void can_bootstrap_a_host()
        {
            var input = new NetCoreInput
            {
#if NETCOREAPP2_2
                HostBuilder = new WebHostBuilder()
                    .UseServer(new NulloServer())
                    .UseStartup<EmptyStartup>()
                #else
                HostBuilder = new HostBuilder()
#endif
            };

            using (var host = input.BuildHost())
            {
                host.ShouldNotBeNull();
            }
        }

        [Fact]
        public void modify_the_environment()
        {
            var input = new NetCoreInput
            {
#if NETCOREAPP2_2
                HostBuilder = new WebHostBuilder()
                    .UseServer(new NulloServer())
                    .UseStartup<EmptyStartup>(),
#else
                HostBuilder = new HostBuilder(),
#endif
                EnvironmentFlag = "Weird"
            };
            
            using (var host = input.BuildHost())
            {
                host.Services.GetRequiredService<IHostingEnvironment>()
                    .EnvironmentName.ShouldBe("Weird");
            }
        }

        [Fact]
        public void modify_configuration_items()
        {
            var input = new NetCoreInput
            {
#if NETCOREAPP2_2
                HostBuilder = new WebHostBuilder()
                    .UseServer(new NulloServer())
                    .UseStartup<EmptyStartup>(),    
                #else
                HostBuilder = new HostBuilder(),
#endif
                ConfigFlag = new Dictionary<string, string>{{"direction", "south"}, {"color", "orange"}}
            };
            
            using (var host = input.BuildHost())
            {
                var config = host.Services.GetRequiredService<IConfiguration>();
                
                config["direction"].ShouldBe("south");
                config["color"].ShouldBe("orange");
            }
        }
    }

    public class EmptyStartup
    {
        public void Configure()
        {
            
        }
    }

    public class NulloServer : IServer
    {
        public void Dispose()
        {
            
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();
    }
}