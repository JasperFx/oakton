using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Baseline;
using Baseline.Reflection;
using Microsoft.AspNetCore.Hosting;
using Oakton.Discovery;

namespace Oakton.AspNetCore
{
    public static class CommandLineHostingExtensions
    {
        /// <summary>
        /// Execute the extended Oakton command line support for your configured WebHostBuilder.
        /// This method would be called within the Task&lt;int&gt; Program.Main(string[] args) method
        /// of your AspNetCore application
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task<int> RunOaktonCommands(this IWebHostBuilder builder, string[] args)
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
            // SAMPLE: using-extension-assemblies
            return CommandExecutor.For(factory =>
            {
                factory.RegisterCommands(typeof(RunCommand).GetTypeInfo().Assembly);
                if (applicationAssembly != null) factory.RegisterCommands(applicationAssembly);

                // This method will direct the CommandFactory to go look for extension
                // assemblies with Oakton commands
                factory.RegisterCommandsFromExtensionAssemblies();

                factory.ConfigureRun = cmd =>
                {
                    if (cmd.Input is AspNetCoreInput) cmd.Input.As<AspNetCoreInput>().WebHostBuilder = source;
                };
            });
            // ENDSAMPLE
        }
        


    }

}