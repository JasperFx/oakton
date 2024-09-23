using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton;

namespace WorkerService
{
    #region sample_using_ihost_activation

    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CreateHostBuilder(args)
                .RunOaktonCommands(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // This is a little old-fashioned, but still valid .NET core code:
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
    }

    #endregion
}