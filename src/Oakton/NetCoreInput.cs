using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JasperFx.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Oakton;

#nullable disable annotations // FIXME

public class NetCoreInput : IHostBuilderInput
{
    [Description("Overwrite individual configuration items")]
    public Dictionary<string, string> ConfigFlag = new();

    [Description("Use to override the ASP.Net Environment name")]
    public string EnvironmentFlag { get; set; }

    [Description("Write out much more information at startup and enables console logging")]
    public bool VerboseFlag { get; set; }

    [Description("Override the log level")]
    public LogLevel? LogLevelFlag { get; set; }

    [IgnoreOnCommandLine] public Assembly ApplicationAssembly { get; set; }

    /// <summary>
    ///     The IHostBuilder configured by your application. Can be used to build or start
    ///     up the application
    /// </summary>
    [IgnoreOnCommandLine]
    public IHostBuilder HostBuilder { get; set; }

    public virtual void ApplyHostBuilderInput()
    {
        #region sample_what_the_cli_is_doing

        // The --log-level flag value overrides your application's
        // LogLevel
        if (LogLevelFlag.HasValue)
        {
            AnsiConsole.MarkupLine($"[gray]Overwriting the minimum log level to {LogLevelFlag.Value}[/]");
            try
            {
                if (HostBuilder is PreBuiltHostBuilder builder)
                {
                    var options = builder.Host.Services.GetService(typeof(LoggerFilterOptions)) as LoggerFilterOptions;
                    options ??=
                        (builder.Host.Services.GetService(typeof(IOptionsMonitor<LoggerFilterOptions>)) as
                            IOptionsMonitor<LoggerFilterOptions>)
                        ?.CurrentValue;

                    if (options != null)
                    {
                        options.MinLevel = LogLevel.Error;
                    }
                }
                else
                {
                    HostBuilder.ConfigureLogging(x => x.SetMinimumLevel(LogLevelFlag.Value));
                }
            }
            catch (Exception)
            {
                AnsiConsole.Markup("[gray]Unable to override the logging level[/]");
            }
        }

        if (VerboseFlag)
        {
            Console.WriteLine("Verbose flag is on.");

            // The --verbose flag adds console and
            // debug logging, as well as setting
            // the minimum logging level down to debug
            HostBuilder.ConfigureLogging(x => { x.SetMinimumLevel(LogLevel.Debug); });
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

        #endregion
    }

    public IHost BuildHost()
    {
        ApplyHostBuilderInput();
        return HostBuilder.Build();
    }
}