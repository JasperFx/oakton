using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;
using Oakton.Descriptions;
using Oakton.Environment;
using Oakton.Resources;

namespace EnvironmentCheckDemonstrator
{
    class Program
    {
        #region sample_extending_describe
        static Task<int> Main(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    
                    for (int i = 0; i < 5; i++)
                    {
                        services.AddSingleton<IEnvironmentCheck>(new GoodEnvironmentCheck(i + 1));
                        services.AddSingleton<IEnvironmentCheck>(new BadEnvironmentCheck(i + 1));


                        services.AddSingleton<IStatefulResource>(new FakeResource("Database", "Db " + (i + 1)));
                    }

                    services.AddSingleton<IStatefulResource>(new FakeResource("Bad", "Blows Up")
                    {
                        Failure = new DivideByZeroException()
                    });
                    
                    services.CheckEnvironment("Inline, async check", async (services, token) =>
                    {
                        await Task.Delay(1.Milliseconds(), token);

                        throw new Exception("I failed!");
                    });
                    
                    
                    // This is an example of adding custom
                    // IDescriptionSystemPart types to your
                    // application that can participate in
                    // the describe output
                    services.AddDescription<Describer1>();
                    services.AddDescription<Describer2>();
                    services.AddDescription<Describer3>();

                })
                .RunOaktonCommands(args);
        }
        #endregion
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
