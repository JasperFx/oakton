using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oakton;
using Oakton.Commands;
using Oakton.Environment;

namespace Net5WebApi
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CreateHostBuilder(args).RunOaktonCommands(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    [Description("Run job1", Name = "job1")]
    public class Job1Command : OaktonAsyncCommand<RunInput>
    {
        public IHost Host { get; private set; }

        public override async Task<bool> Execute(RunInput input)
        {
            input.ApplyHostBuilderInput();
            input.HostBuilder.ConfigureServices(services => services.AddHostedService<Job1>());

            using (Host = input.BuildHost())
            {
                if (input.CheckFlag)
                {
                    var report = await EnvironmentChecker.ExecuteAllEnvironmentChecks(Host.Services);
                    report.Assert();
                }

                await Host.RunAsync();
            }


            return true;
        }
    }

    public class Job1 : BackgroundService
    {
        private readonly ILogger<Job1> _logger;

        public Job1(ILogger<Job1> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                _logger.LogInformation("Job1 running...");
                await Task.Delay(1000);
            } while (!stoppingToken.IsCancellationRequested);
        }
    }

    [Description("Run job2", Name = "job2")]
    public class Job2Command : OaktonAsyncCommand<RunInput>
    {
        public IHost Host { get; private set; }

        public override async Task<bool> Execute(RunInput input)
        {
            input.ApplyHostBuilderInput();
            input.HostBuilder.ConfigureServices(services => services.AddHostedService<Job1>());

            using (Host = input.BuildHost())
            {
                if (input.CheckFlag)
                {
                    var report = await EnvironmentChecker.ExecuteAllEnvironmentChecks(Host.Services);
                    report.Assert();
                }

                await Host.RunAsync();
            }


            return true;
        }
    }

    public class Job2 : BackgroundService
    {
        private readonly ILogger<Job2> _logger;

        public Job2(ILogger<Job2> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                _logger.LogInformation("Job2 running...");
                await Task.Delay(1000);
            } while (!stoppingToken.IsCancellationRequested);
        }
    }
}