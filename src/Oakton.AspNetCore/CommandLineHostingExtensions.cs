using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Baseline;
using Baseline.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oakton.AspNetCore.Environment;

namespace Oakton.AspNetCore
{
    public static class CommandLineHostingExtensions
    {
        public static Task ExecuteAllEnvironmentChecks(this IWebHost host, CancellationToken token = default(CancellationToken))
        {
            return host.Services.ExecuteAllEnvironmentChecks(token);
        }
        
        public static async Task ExecuteAllEnvironmentChecks(this IServiceProvider services, CancellationToken token = default(CancellationToken))
        {
            var exceptions = new List<Exception>();


            var checks = services.discoverChecks().ToArray();
            if (!checks.Any()) return;
            
            ConsoleWriter.Write("Running Environment Checks");

            for (int i = 0; i < checks.Length; i++)
            {
                try
                {
                    await checks[i].Assert(services, token);
                    ConsoleWriter.Write(ConsoleColor.Green, $"{(i + 1).ToString().PadLeft(4)}.) Success: {checks[i].Description}");
                }
                catch (Exception e)
                {
                    ConsoleWriter.Write(ConsoleColor.Red, $"{(i + 1).ToString().PadLeft(4)}.) Failed: {checks[i].Description}");
                    ConsoleWriter.Write(ConsoleColor.Yellow, e.ToString());
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("Environment Checks Failed!", exceptions);
            }

        }

        private static IEnumerable<IEnvironmentCheck> discoverChecks(this IServiceProvider services)
        {
            foreach (var check in services.GetServices<IEnvironmentCheck>())
            {
                yield return check;
            }

            foreach (var factory in services.GetServices<IEnvironmentCheckFactory>())
            {
                foreach (var check in factory.Build())
                {
                    yield return check;
                }
            }
        }
        
        public static Task<int> RunCommands(this IWebHostBuilder builder, string[] args)
        {
            return Execute(builder, null, args);
        }
        
        internal static Task<int> Execute(IWebHostBuilder runtimeSource, Assembly applicationAssembly, string[] args)
        {
            if (args == null || args.Length == 0 || args[0].StartsWith("-"))
                args = new[] {"run"}.Concat(args ?? new string[0]).ToArray();

            if (applicationAssembly == null)
            {
                var name = runtimeSource.GetSetting(WebHostDefaults.ApplicationKey);
                if (name.IsNotEmpty())
                {
                    applicationAssembly = Assembly.Load(name);
                }
            }

            return buildExecutor(runtimeSource, applicationAssembly).ExecuteAsync(args);
        }


        private static CommandExecutor buildExecutor(IWebHostBuilder source, Assembly applicationAssembly)
        {
            return CommandExecutor.For(factory =>
            {
                factory.RegisterCommands(typeof(RunCommand).GetTypeInfo().Assembly);
                if (applicationAssembly != null) factory.RegisterCommands(applicationAssembly);

                foreach (var assembly in FindExtensionAssemblies(applicationAssembly)) factory.RegisterCommands(assembly);

                factory.ConfigureRun = cmd =>
                {
                    if (cmd.Input is AspNetCoreInput) cmd.Input.As<AspNetCoreInput>().WebHostBuilder = source;
                };
            });
        }
        
        internal static Assembly[] FindExtensionAssemblies(Assembly applicationAssembly)
        {
            return AssemblyFinder
                .FindAssemblies(txt => { }, false)
                .Concat(AppDomain.CurrentDomain.GetAssemblies())
                .Distinct()
                .Where(a => a.HasAttribute<OaktonCommandAssemblyAttribute>())
                .ToArray();
        }

    }

}