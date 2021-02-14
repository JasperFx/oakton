using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Baseline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Oakton
{
    public class NetCoreInput
    {

        [Description("Use to override the ASP.Net Environment name")]
        public string EnvironmentFlag { get; set; }

        [Description("Write out much more information at startup and enables console logging")]
        public bool VerboseFlag { get; set; }

        [Description("Override the log level")]
        public LogLevel? LogLevelFlag { get; set; }
        
        [Description("Overwrite individual configuration items")]
        public Dictionary<string, string> ConfigFlag = new Dictionary<string, string>();
        
        [IgnoreOnCommandLine] public IHostBuilder HostBuilder { get; set; }

        [IgnoreOnCommandLine] public Assembly ApplicationAssembly { get; set; }

        public IHost BuildHost() 
        {
            // SAMPLE: what-the-cli-is-doing

            // The --log-level flag value overrides your application's
            // LogLevel
            if (LogLevelFlag.HasValue)
            {
                Console.WriteLine($"Overwriting the minimum log level to {LogLevelFlag.Value}");
                HostBuilder.ConfigureLogging(x => x.SetMinimumLevel(LogLevelFlag.Value));
            }

            if (VerboseFlag)
            {
                Console.WriteLine("Verbose flag is on.");

                // The --verbose flag adds console and
                // debug logging, as well as setting
                // the minimum logging level down to debug
                HostBuilder.ConfigureLogging(x =>
                {
                    x.SetMinimumLevel(LogLevel.Debug);
                });
            }

            // The --environment flag is used to set the environment
            // property on the IHostedEnvironment within your system
            if (EnvironmentFlag.IsNotEmpty())
            {
                Console.WriteLine($"Overwriting the environment to `{EnvironmentFlag}`");
                HostBuilder.UseEnvironment(EnvironmentFlag);
            }

            if (ConfigFlag.Any())
            {
                HostBuilder.ConfigureAppConfiguration(c => c.AddInMemoryCollection(ConfigFlag));
            }
            // ENDSAMPLE

            return HostBuilder.Build();
        }
    }
}