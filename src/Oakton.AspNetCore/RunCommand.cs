using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server.Features;
#else
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.DependencyInjection;

using Oakton.AspNetCore.Environment;

namespace Oakton.AspNetCore
{
    public class RunInput : NetCoreInput
    {
        [Description("Run the environment checks before starting the host")]
        public bool CheckFlag { get; set; }
    }
    
    [Description("Runs the configured AspNetCore application")]
    public class RunCommand : OaktonCommand<RunInput>
    {
#if NETSTANDARD2_0
        public IWebHost Host { get; private set; }
        #else
        public IHost Host { get; private set; }
#endif

        public ManualResetEventSlim Reset { get; } = new ManualResetEventSlim();
        public ManualResetEventSlim Started { get; } = new ManualResetEventSlim();

        public void Shutdown()
        {
            Console.WriteLine();
            Console.WriteLine("Application is shutting down...");
            Host.Dispose();
            Reset.Set();
        }
        
        public override bool Execute(RunInput input)
        {
            Host = input.BuildHost();

            if (input.CheckFlag)
            {
                EnvironmentChecker.ExecuteAllEnvironmentChecks(Host.Services)
                    .GetAwaiter().GetResult()
                    .Assert();
            }


            var assembly = typeof(RunCommand).GetTypeInfo().Assembly;
            AssemblyLoadContext.GetLoadContext(assembly).Unloading += context => Shutdown();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown();
                eventArgs.Cancel = true;
            };

            using (Host)
            {
                Host.Start();

                Started.Set();

                var shutdownMessage = "Press CTRL + C to quit";
                
                // TODO -- do this with a flag
                //Console.WriteLine("Running all environment checks...");
                //host.ExecuteAllEnvironmentChecks();

                IHostingEnvironment service = Host.Services.GetService<IHostingEnvironment>();

                Console.WriteLine("Hosting environment: " + service.EnvironmentName);
                Console.WriteLine("Content root path: " + service.ContentRootPath);
#if NETSTANDARD2_0
                ICollection<string> addresses = Host.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
                if (addresses != null)
                {
                    foreach (string str in addresses)
                        Console.WriteLine("Now listening on: " + str);
                }
#endif
                
                if (!string.IsNullOrEmpty(shutdownMessage))
                    Console.WriteLine(shutdownMessage);


                Reset.Wait();
                
                Host.StopAsync().GetAwaiter().GetResult();
            }


            return true;
        }
    }
}