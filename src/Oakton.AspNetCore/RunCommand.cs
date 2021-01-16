using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

using Microsoft.Extensions.Hosting;
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
        public IHost Host { get; private set; }

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

                var service = Host.Services.GetService<IHostEnvironment>();

                Console.WriteLine("Hosting environment: " + service.EnvironmentName);
                Console.WriteLine("Content root path: " + service.ContentRootPath);
                
                if (!string.IsNullOrEmpty(shutdownMessage))
                    Console.WriteLine(shutdownMessage);


                Reset.Wait();
                
                Host.StopAsync().GetAwaiter().GetResult();
            }


            return true;
        }
    }
}