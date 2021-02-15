using System;
using System.Threading;
using System.Threading.Tasks;
using Baseline.Dates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using Oakton.Descriptions;
using Oakton.Environment;

namespace EnvironmentCheckDemonstrator
{
    class Program
    {
        static Task<int> Main(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        services.AddSingleton<IEnvironmentCheck>(new GoodEnvironmentCheck(i + 1));
                        services.AddSingleton<IEnvironmentCheck>(new BadEnvironmentCheck(i + 1));
                        
                    }
                    
                    services.AddDescription<Describer1>();
                    services.AddDescription<Describer2>();
                    services.AddDescription<Describer3>();

                })
                .RunOaktonCommands(args);
        }
    }

    public class GoodEnvironmentCheck : IEnvironmentCheck
    {
        private readonly int _number;

        public GoodEnvironmentCheck(int number)
        {
            _number = number;
        }

        public string Description => "Good #" + _number;
        public async Task Assert(IServiceProvider services, CancellationToken cancellation)
        {
            await Task.Delay(3.Seconds(), cancellation);
        }
    }
    
    public class BadEnvironmentCheck : IEnvironmentCheck
    {
        private readonly int _number;

        public BadEnvironmentCheck(int number)
        {
            _number = number;
        }

        public string Description => "Bad #" + _number;
        public async Task Assert(IServiceProvider services, CancellationToken cancellation)
        {
            await Task.Delay(3.Seconds(), cancellation);
            throw new DivideByZeroException("Boom!");
        }
    }
}