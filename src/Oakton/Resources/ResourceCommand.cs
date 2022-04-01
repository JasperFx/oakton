using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Baseline;
using Baseline.Dates;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Rendering;
using StringExtensions = Baseline.StringExtensions;

namespace Oakton.Resources
{
    public class ResourceCommand : OaktonAsyncCommand<ResourceInput>
    {
        public ResourceCommand()
        {
            Usage("Ensure all stateful resources are set up").NoArguments();
            Usage("Execute an action against all resources").Arguments(x => x.Action);
        }

        public override async Task<bool> Execute(ResourceInput input)
        {
            var cancellation = input.TokenSource.Token;
            using var host = input.BuildHost();
            var resources = await AllResources(host.Services, cancellation);
            if (input.NameFlag.IsNotEmpty())
            {
                resources = resources.Where(x => StringExtensions.EqualsIgnoreCase(x.Name, input.NameFlag)).ToList();
            }

            if (input.TypeFlag.IsNotEmpty())
            {
                resources = resources.Where(x => x.Type.EqualsIgnoreCase(input.TypeFlag)).ToList();
            }

            if (!resources.Any())
            {
                AnsiConsole.WriteLine("[gray]No matching resources.[/]");
                return true;
            }

            switch (input.Action)
            {
                case ResourceAction.setup:
                    return await ExecuteOnEach(resources, cancellation, "Setting up resources...", r => r.Setup(cancellation));
                
                case ResourceAction.teardown:
                    return await ExecuteOnEach(resources, cancellation, "Tearing down resources...", r => r.Setup(cancellation));
                
                case ResourceAction.check:
                    return await ExecuteOnEach(resources, cancellation, "Checking resources...", r => r.Setup(cancellation));
                
                case ResourceAction.status:
                    return await ExecuteOnEach(resources, cancellation, "Determining resource status...", r => r.Check(cancellation));
                
                case ResourceAction.clear:
                    return await ExecuteOnEach(resources, cancellation, "Clearing stateful resources...", r => r.Check(cancellation));
            }

            return false;
        }


        internal async Task<bool> ExecuteOnEach(IList<IStatefulResource> resources, CancellationToken token, string progressTitle, Func<IStatefulResource, Task<object>> execution)
        {
            var exceptions = new List<Exception>();
            var table = new Table();
            table.AddColumns("Resource", "Status");

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
                        if (status is IRenderable r)
                        {
                            table.AddRow(new Markup($"[green]{resource}[/]"), r);
                        }
                        else
                        {
                            table.AddRow($"[green]{resource}[/]", status?.ToString() ?? string.Empty);
                        }
                    }
                    catch (Exception e)
                    {
                        table.AddRow($"[red]{resource}[/]", e.Message);
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
                AnsiConsole.WriteLine("[bold red]Timed out![/]");
                return false;
            }
            
            AnsiConsole.Write(table);

            if (exceptions.Any())
            {
                AnsiConsole.WriteLine("Exceptions:");
                foreach (var exception in exceptions)
                {
                    AnsiConsole.WriteException(exception);
                    AnsiConsole.WriteLine();
                }
                
                return false;
            }

            return true;
        }
        
        internal async Task<IList<IStatefulResource>> AllResources(IServiceProvider services,
            CancellationToken cancellation)
        {
            var list = services.GetServices<IStatefulResource>().ToList();
            foreach (var source in services.GetServices<IStatefulResourceSource>())
            {
                var sources = await source.FindResources(cancellation);
                list.AddRange(sources);
            }

            return list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }
        
    }
}