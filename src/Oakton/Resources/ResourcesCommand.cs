using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Baseline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Oakton.Resources
{
    [Description("Check, setup, or teardown stateful resources of this system")]
    public class ResourcesCommand : OaktonAsyncCommand<ResourceInput>
    {
        public ResourcesCommand()
        {
            Usage("Ensure all stateful resources are set up").NoArguments();
            Usage("Execute an action against all resources").Arguments(x => x.Action);
        }

        public override async Task<bool> Execute(ResourceInput input)
        {
            var cancellation = input.TokenSource.Token;
            using var host = input.BuildHost();
            var resources = await FilterResources(input, host, cancellation);

            if (!resources.Any())
            {
                AnsiConsole.MarkupLine("[gray]No matching resources.[/]");
                return true;
            }

            var allGood = new Markup("[green]Success.[/]");

            return input.Action switch
            {
                ResourceAction.setup => await ExecuteOnEach(resources, cancellation, "Setting up resources...", r =>
                {
                    r.Setup(cancellation);
                    return r.DetermineStatus(cancellation);
                }),

                ResourceAction.teardown => await ExecuteOnEach(resources, cancellation, "Tearing down resources...",
                    async r =>
                    {
                        await r.Teardown(cancellation);
                        return allGood;
                    }),

                ResourceAction.statistics => await ExecuteOnEach(resources, cancellation,
                    "Determining resource status...",
                    r => r.DetermineStatus(cancellation)),

                ResourceAction.check => await ExecuteOnEach(resources, cancellation, "Checking up on resources...",
                    async r =>
                    {
                        await r.Check(cancellation);
                        return allGood;
                    }),

                ResourceAction.clear => await ExecuteOnEach(resources, cancellation, "Clearing resources...", async r =>
                {
                    await r.ClearState(cancellation);
                    return allGood;
                }),

                _ => false
            };
        }

        public async Task<IList<IStatefulResource>> FilterResources(ResourceInput input, IHost host, CancellationToken cancellation)
        {
            var resources = await AllResources(host.Services, cancellation);
            if (input.NameFlag.IsNotEmpty())
            {
                resources = resources.Where(x => x.Name.EqualsIgnoreCase(input.NameFlag)).ToList();
            }

            if (input.TypeFlag.IsNotEmpty())
            {
                resources = resources.Where(x => x.Type.EqualsIgnoreCase(input.TypeFlag)).ToList();
            }

            return resources;
        }


        internal async Task<bool> ExecuteOnEach(IList<IStatefulResource> resources, CancellationToken token,
            string progressTitle, Func<IStatefulResource, Task<IRenderable>> execution)
        {
            var exceptions = new List<Exception>();
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumns("Resource", "Type", "Status");
            table.Columns[2].NoWrap();

            var timedout = false;

            await AnsiConsole.Progress().StartAsync(async c =>
            {
                var task = c.AddTask($"[bold]{progressTitle}[/]", new ProgressTaskSettings
                {
                    MaxValue = resources.Count
                });

                foreach (var resource in resources)
                {
                    if (token.IsCancellationRequested)
                    {
                        timedout = true;
                        break;
                    }

                    try
                    {
                        var status = await execution(resource);
                        table.AddRow(new Markup(resource.Name), new Markup(resource.Type), status);
                    }
                    catch (Exception e)
                    {
                        table.AddRow(new Markup(resource.Name), new Markup(resource.Type), new Markup($"[red]{e.Message}[/]"));
                        exceptions.Add(e);
                    }
                    finally
                    {
                        task.Increment(1);
                    }
                }
            });

            AnsiConsole.WriteLine();

            if (timedout)
            {
                AnsiConsole.MarkupLine("[bold red]Timed out![/]");
                return false;
            }

            AnsiConsole.Write(table);

            if (!exceptions.Any())
            {
                return true;
            }

            AnsiConsole.WriteLine("Exceptions:");
            foreach (var exception in exceptions)
            {
                AnsiConsole.WriteException(exception);
                AnsiConsole.WriteLine();
            }

            return false;
        }

        internal async Task<IList<IStatefulResource>> AllResources(IServiceProvider services,
            CancellationToken cancellation)
        {
            var list = services.GetServices<IStatefulResource>().ToList();
            foreach (var source in services.GetServices<IStatefulResourceSource>())
            {
                var sources = source.FindResources();
                list.AddRange(sources);
            }

            return list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }
    }
}