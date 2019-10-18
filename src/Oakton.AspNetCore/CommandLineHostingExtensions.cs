using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Baseline;
using Oakton.AspNetCore.Internal;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Hosting;
#else
using Microsoft.Extensions.Hosting;
#endif


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
#if NETSTANDARD2_0
        public static Task<int> RunOaktonCommands(this IWebHostBuilder builder, string[] args)
        {
            return Execute(builder, null, args);
        }
#else
        public static Task<int> RunOaktonCommands(this IHostBuilder builder, string[] args)
        {
            return Execute(builder, null, args);
        }  
#endif
        
        
        
#if NETSTANDARD2_0
        internal static Task<int> Execute(IWebHostBuilder runtimeSource, Assembly applicationAssembly, string[] args)
        #else
        internal static Task<int> Execute(IHostBuilder runtimeSource, Assembly applicationAssembly, string[] args)
#endif
        {
            // Workaround for IISExpress / VS2019 erroneously putting crap arguments
            args = args.FilterLauncherArgs();
            
            if (args == null || args.Length == 0 || args[0].StartsWith("-"))
                args = new[] {"run"}.Concat(args ?? new string[0]).ToArray();

            if (applicationAssembly == null)
            {
//                var name = runtimeSource.GetSetting(WebHostDefaults.ApplicationKey);
//                if (name.IsNotEmpty())
//                {
//                    applicationAssembly = Assembly.Load(name);
//                }
            }

            return buildExecutor(runtimeSource, applicationAssembly).ExecuteAsync(args);
        }


#if NETSTANDARD2_0
        private static CommandExecutor buildExecutor(IWebHostBuilder source, Assembly applicationAssembly)
#else
        private static CommandExecutor buildExecutor(IHostBuilder source, Assembly applicationAssembly)
#endif
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
                    if (cmd.Input is NetCoreInput) cmd.Input.As<NetCoreInput>().HostBuilder = source;
                };
            });
            // ENDSAMPLE
        }
        


    }

}