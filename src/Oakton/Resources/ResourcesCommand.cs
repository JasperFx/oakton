using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

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
        AnsiConsole.Write(
            new FigletText("Oakton"){Justification = Justify.Left});

        var cancellation = input.TokenSource.Token;
        using var host = input.BuildHost();
        var resources = FindResources(input, host);

        if (!resources.Any())
        {
            AnsiConsole.MarkupLine("[gray]No matching resources.[/]");
            return true;
        }

        var allGood = new Markup("[green]Success.[/]");

        return input.Action switch
        {
            ResourceAction.setup => await ExecuteOnEach("Resource Setup", resources, cancellation,
                "Setting up resources...", async r =>
                {
                    await r.Setup(cancellation);
                    return await r.DetermineStatus(cancellation);
                }),

            ResourceAction.teardown => await ExecuteOnEach("Resource Teardown", resources, cancellation,
                "Tearing down resources...",
                async r =>
                {
                    await r.Teardown(cancellation);
                    return allGood;
                }),

            ResourceAction.statistics => await ExecuteOnEach("Resource Statistics", resources, cancellation,
                "Determining resource status...",
                r => r.DetermineStatus(cancellation)),

            ResourceAction.check => await ExecuteOnEach("Resource Checks", resources, cancellation,
                "Checking up on resources...",
                async r =>
                {
                    await r.Check(cancellation);
                    return allGood;
                }),

            ResourceAction.clear => await ExecuteOnEach("Clearing Resource State", resources, cancellation,
                "Clearing resources...", async r =>
                {
                    await r.ClearState(cancellation);
                    return allGood;
                }),

            ResourceAction.list => listAll(resources),

            _ => false
        };
    }

    private bool listAll(IList<IStatefulResource> statefulResources)
    {
        var table = new Table();
        table.AddColumns("Resource Type", "Resource Name");
        foreach (var resource in statefulResources) table.AddRow(resource.Type, resource.Name);

        AnsiConsole.Write(table);

        return true;
    }

    public IList<IStatefulResource> FindResources(ResourceInput input, IHost host)
    {
        return FindResources(host.Services, input.TypeFlag, input.NameFlag);
    }

    internal async Task<bool> ExecuteOnEach(string heading, IList<IStatefulResource> resources, CancellationToken token,
        string progressTitle, Func<IStatefulResource, Task<IRenderable>> execution)
    {
        var exceptions = new List<Exception>();
        var records = new List<ResourceRecord>();

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
                    var record = new ResourceRecord { Resource = resource, Status = status };
                    records.Add(record);
                }
                catch (Exception e)
                {
                    AnsiConsole.WriteException(e);

                    var record = new ResourceRecord { Resource = resource, Status = new Markup("[red]Failed![/]") };
                    records.Add(record);

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

        var groups = records.GroupBy(x => x.Resource.Type);
        var tree = new Tree(heading);
        tree.Guide = TreeGuide.BoldLine;
        foreach (var group in groups)
        {
            var groupNode = tree.AddNode(group.Key);
            foreach (var record in group) groupNode.AddNode(record.Resource.Name).AddNode(record.Status);
        }

        AnsiConsole.Write(tree);


        return !exceptions.Any();
    }

    internal static IList<IStatefulResource> FindResources(IServiceProvider services, string typeName,
        string resourceName)
    {
        var list = services.GetServices<IStatefulResource>().ToList();
        foreach (var source in services.GetServices<IStatefulResourceSource>())
        {
            var sources = source.FindResources();
            list.AddRange(sources);
        }

        if (resourceName.IsNotEmpty())
        {
            list = list.Where(x => x.Name.EqualsIgnoreCase(resourceName)).ToList();
        }

        if (typeName.IsNotEmpty())
        {
            list = list.Where(x => x.Type.EqualsIgnoreCase(typeName)).ToList();
        }

        // Initial sort
        list =  list.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();

        if (!list.OfType<IStatefulResourceWithDependencies>().Any()) return list;

        IEnumerable<IStatefulResource> FindDependencies(IStatefulResource resource) =>
            resource is IStatefulResourceWithDependencies x
                ? x.FindDependencies(list)
                : Array.Empty<IStatefulResource>();

        // Again on dependencies
        return list.TopologicalSort(FindDependencies).ToList();
    }

    internal class ResourceRecord
    {
        public IStatefulResource Resource { get; set; }
        public IRenderable Status { get; set; }
    }
}