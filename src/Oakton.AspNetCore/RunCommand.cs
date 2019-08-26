using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.AspNetCore
{
    [Description("Runs the configured AspNetCore application")]
    public class RunCommand : OaktonCommand<AspNetCoreInput>
    {
        public override bool Execute(AspNetCoreInput input)
        {
            var host = input.BuildHost();

            var done = new ManualResetEventSlim(false);
            var cts = new CancellationTokenSource();

            try
            {
                void shutdown()
                {
                    if (!cts.IsCancellationRequested)
                    {
                        Console.WriteLine("Application is shutting down...");
                        host.Dispose();
                        cts.Cancel();
                    }

                    done.Set();
                }

                var assembly = typeof(RunCommand).GetTypeInfo().Assembly;
                AssemblyLoadContext.GetLoadContext(assembly).Unloading += context => shutdown();

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    shutdown();
                    eventArgs.Cancel = true;
                };

                using (host)
                {
                    host.Start();

                    var shutdownMessage = "Press CTRL + C to quit";
                    
                    // TODO -- do this with a flag
                    //Console.WriteLine("Running all environment checks...");
                    //host.ExecuteAllEnvironmentChecks();

                    IHostingEnvironment service = host.Services.GetService<IHostingEnvironment>();

                    Console.WriteLine("Hosting environment: " + service.EnvironmentName);
                    Console.WriteLine("Content root path: " + service.ContentRootPath);
                    ICollection<string> addresses = host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
                    if (addresses != null)
                    {
                        foreach (string str in addresses)
                            Console.WriteLine("Now listening on: " + str);
                    }
                    if (!string.IsNullOrEmpty(shutdownMessage))
                        Console.WriteLine(shutdownMessage);
                    
                        
                    done.Wait(cts.Token);
                }
            }
            finally
            {
                cts?.Dispose();
            }

            return true;
        }
    }
}