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
        public IHost Host { get; private set; }

        public override async Task<bool> Execute(RunInput input)
        {
            using (Host = input.BuildHost())
            {
                if (input.CheckFlag)
                {
                    var report = await EnvironmentChecker.ExecuteAllEnvironmentChecks(Host.Services);
                    report.Assert();
                }

                await Host.RunAsync();
            }


            return true;
        }
    }
}