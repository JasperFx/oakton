using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.Environment;

namespace Oakton.Commands
{
    public class RunInput : NetCoreInput
    {
        [Description("Run the environment checks before starting the host")]
        public bool CheckFlag { get; set; }
    }
    
    [Description("Start and run this .Net application")]
    public class RunCommand : OaktonAsyncCommand<RunInput>
    {
        public async override Task<bool> Execute(RunInput input)
        {
            using (var host = input.BuildHost())
            {
                if (input.CheckFlag)
                    (await EnvironmentChecker.ExecuteAllEnvironmentChecks(host.Services)).Assert();
                
                var reset = new ManualResetEventSlim();
                AssemblyLoadContext.GetLoadContext(typeof (RunCommand).GetTypeInfo().Assembly).Unloading += (Action<AssemblyLoadContext>) (context => reset.Set());
                Console.CancelKeyPress += (ConsoleCancelEventHandler) ((sender, eventArgs) =>
                {
                    reset.Set();
                    eventArgs.Cancel = true;
                });
            
                await host.StartAsync();
                reset.Wait();
                await host.StopAsync();
            }
            return true;
        }
    }
}