using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Baseline;
using Baseline.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Oakton.AspNetCore
{
    public static class HostBuilderExtensions
    {
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
                    if (cmd.Input is NetCoreInput) cmd.Input.As<NetCoreInput>().WebHostBuilder = source;
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

        public static void ExecuteAllEnvironmentChecks(this IWebHost host)
        {
            throw new NotImplementedException();
        }
    }


    // SAMPLE: LamarInput

    // ENDSAMPLE
}