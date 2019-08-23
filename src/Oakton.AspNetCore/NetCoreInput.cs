using System;
using System.Reflection;
using Baseline;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Oakton.AspNetCore
{
    public class NetCoreInput
    {

        [Description("Use to override the ASP.Net Environment name")]
        public string EnvironmentFlag { get; set; }

        [Description("Write out much more information at startup and enables console logging")]
        public bool VerboseFlag { get; set; }

        [Description("Override the log level")]
        public LogLevel? LogLevelFlag { get; set; }

        [IgnoreOnCommandLine] public IWebHostBuilder WebHostBuilder { get; set; }

        [IgnoreOnCommandLine] public Assembly ApplicationAssembly { get; set; }

        public IWebHost BuildHost()
        {
            // SAMPLE: what-the-cli-is-doing

            // The --log-level flag value overrides your application's
            // LogLevel
            if (LogLevelFlag.HasValue)
            {
                Console.WriteLine($"Overwriting the minimum log level to {LogLevelFlag.Value}");
                WebHostBuilder.ConfigureLogging(x => x.SetMinimumLevel(LogLevelFlag.Value));
            }

            if (VerboseFlag)
            {
                Console.WriteLine("Verbose flag is on.");

                // The --verbose flag adds console and
                // debug logging, as well as setting
                // the minimum logging level down to debug
                WebHostBuilder.ConfigureLogging(x =>
                {
                    x.SetMinimumLevel(LogLevel.Debug);
                });
            }

            // The --environment flag is used to set the environment
            // property on the IHostedEnvironment within your system
            if (EnvironmentFlag.IsNotEmpty())
            {
                Console.WriteLine($"Overwriting the environment to `{EnvironmentFlag}`");
                WebHostBuilder.UseEnvironment(EnvironmentFlag);
            }
            // ENDSAMPLE

            return WebHostBuilder.Build();
        }
    }
}