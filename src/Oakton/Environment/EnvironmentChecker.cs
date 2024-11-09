using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Oakton.Resources;
using Spectre.Console;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

/// <summary>
///     Executes the environment checks registered in an IoC container
/// </summary>
public static class EnvironmentChecker
{
    public static async Task<EnvironmentCheckResults> ExecuteAllEnvironmentChecks(IServiceProvider services,
        CancellationToken token = default)
    {
        var results = new EnvironmentCheckResults();

        var checks = services.discoverChecks().ToArray();
        if (!checks.Any())
        {
            AnsiConsole.WriteLine("No environment checks.");
            return results;
        }

        await AnsiConsole.Progress().StartAsync(async c =>
        {
            var task = c.AddTask("[bold]Running Environment Checks[/]", new ProgressTaskSettings
            {
                MaxValue = checks.Length
            });

            for (var i = 0; i < checks.Length; i++)
            {
                var check = checks[i];

                try
                {
                    await check.Assert(services, token);

                    AnsiConsole.MarkupLine(
                        $"[green]{(i + 1).ToString().PadLeft(4)}.) Success: {check.Description}[/]");

                    results.RegisterSuccess(check.Description);
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine(
                        $"[red]{(i + 1).ToString().PadLeft(4)}.) Failed: {check.Description}[/]");
                    AnsiConsole.WriteException(e);

                    results.RegisterFailure(check.Description, e);
                }
                finally
                {
                    task.Increment(1);
                }
            }

            task.StopTask();
        });

        return results;
    }

    private static IEnumerable<IEnvironmentCheck> discoverChecks(this IServiceProvider services)
    {
        foreach (var check in services.GetServices<IEnvironmentCheck>()) yield return check;

        foreach (var factory in services.GetServices<IEnvironmentCheckFactory>())
        {
            foreach (var check in factory.Build()) yield return check;
        }

        foreach (var resource in services.GetServices<IStatefulResource>())
            yield return new ResourceEnvironmentCheck(resource);

        foreach (var source in services.GetServices<IStatefulResourceSource>())
        {
            foreach (var resource in source.FindResources()) yield return new ResourceEnvironmentCheck(resource);
        }
    }
}