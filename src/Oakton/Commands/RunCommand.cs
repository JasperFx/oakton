using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Oakton.Environment;

namespace Oakton.Commands;

#nullable disable annotations // FIXME

public class RunInput : NetCoreInput
{
    [Description("Run the environment checks before starting the host")]
    public bool CheckFlag { get; set; }
}

[Description("Start and run this .Net application")]
public class RunCommand : OaktonAsyncCommand<RunInput>
{
    public override async Task<bool> Execute(RunInput input)
    {
        using var host = input.BuildHost();
        if (input.CheckFlag)
        {
            var checks = await EnvironmentChecker.ExecuteAllEnvironmentChecks(host.Services);
            checks.Assert();
        }

        var reset = new ManualResetEventSlim();
        // ReSharper disable once PossibleNullReferenceException
        AssemblyLoadContext.GetLoadContext(typeof(RunCommand).Assembly).Unloading +=
            (Action<AssemblyLoadContext>)(context => reset.Set());
        Console.CancelKeyPress += (ConsoleCancelEventHandler)((sender, eventArgs) =>
        {
            reset.Set();
            eventArgs.Cancel = true;
        });

        var lifetime = host.Services.GetService<IHostApplicationLifetime>();
        lifetime?.ApplicationStopping.Register(() => reset.Set());

        await host.StartAsync();
        reset.Wait();
        await host.StopAsync();
        return true;
    }
}
