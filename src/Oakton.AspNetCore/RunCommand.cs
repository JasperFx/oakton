using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.AspNetCore
{
    [Description("Runs the configured AspNetCore application")]
    public class RunCommand : OaktonAsyncCommand<AspNetCoreInput>
    {
        public IWebHost Host { get; private set; }

        public CancellationTokenSource Cts { get; private set; }
        public TaskCompletionSource<bool> Completion { get; } = new TaskCompletionSource<bool>();

        public async Task Shutdown()
        {
            if (!Cts.IsCancellationRequested)
            {
                Console.WriteLine("Application is shutting down...");
                Host.Dispose();
                Cts.Cancel();
            }

            Cts.Cancel();
            Completion.TrySetResult(true);
        }
        
        public async override Task<bool> Execute(AspNetCoreInput input)
        {
            Host = input.BuildHost();


            Cts = new CancellationTokenSource();



            try
            {
                var assembly = typeof(RunCommand).GetTypeInfo().Assembly;
                AssemblyLoadContext.GetLoadContext(assembly).Unloading += context => Shutdown();

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Shutdown();
                    eventArgs.Cancel = true;
                };

                using (Host)
                {
                    await Host.StartAsync();

                    var shutdownMessage = "Press CTRL + C to quit";
                    
                    // TODO -- do this with a flag
                    //Console.WriteLine("Running all environment checks...");
                    //host.ExecuteAllEnvironmentChecks();

                    IHostingEnvironment service = Host.Services.GetService<IHostingEnvironment>();

                    Console.WriteLine("Hosting environment: " + service.EnvironmentName);
                    Console.WriteLine("Content root path: " + service.ContentRootPath);
                    ICollection<string> addresses = Host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
                    if (addresses != null)
                    {
                        foreach (string str in addresses)
                            Console.WriteLine("Now listening on: " + str);
                    }
                    if (!string.IsNullOrEmpty(shutdownMessage))
                        Console.WriteLine(shutdownMessage);


                    await Completion.Task;
                }
            }
            finally
            {
                Cts?.Dispose();
            }

            return true;
        }
    }
}