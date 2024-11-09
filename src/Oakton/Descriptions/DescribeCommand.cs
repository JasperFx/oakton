using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JasperFx.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace Oakton.Descriptions;

#nullable disable annotations // FIXME

[Description("Writes out a description of your running application to either the console or a file")]
public class DescribeCommand : OaktonAsyncCommand<DescribeInput>
{
    public override async Task<bool> Execute(DescribeInput input)
    {
        using var host = input.BuildHost();

        var config = host.Services.GetRequiredService<IConfiguration>();
        var configurationPreview = new ConfigurationPreview(config);

        var hosting = host.Services.GetService<IHostEnvironment>();
        var about = new AboutThisAppPart(hosting, config);
        var builtInDescribers = new IDescribedSystemPart[] { about, configurationPreview, new ReferencedAssemblies() };

        var factories = host.Services.GetServices<IDescribedSystemPartFactory>();

        var parts = host.Services.GetServices<IDescribedSystemPart>()
            .Concat(factories.SelectMany(x => x.Parts()))
            .Concat(builtInDescribers).ToArray();

        foreach (var partWithServices in parts.OfType<IRequiresServices>()) partWithServices.Resolve(host.Services);

        if (input.ListFlag)
        {
            Console.WriteLine("The registered system parts are");
            foreach (var part in parts) Console.WriteLine("* " + part.Title);

            return true;
        }

        if (input.TitleFlag.IsNotEmpty())
        {
            parts = parts.Where(x => x.Title == input.TitleFlag).ToArray();
        }
        else if (input.InteractiveFlag)
        {
            var prompt = new MultiSelectionPrompt<string>()
                .Title("What part(s) of your application do you wish to view?")
                .PageSize(10)
                .AddChoices(parts.Select(x => x.Title));

            var titles = AnsiConsole.Prompt(prompt);

            parts = parts.Where(x => titles.Contains(x.Title)).ToArray();
        }

        if (!input.SilentFlag)
        {
            await WriteToConsole(parts);
        }

        if (!input.FileFlag.IsNotEmpty())
        {
            return true;
        }

        await using (var stream = new FileStream(input.FileFlag, FileMode.CreateNew, FileAccess.Write))
        {
            var writer = new StreamWriter(stream);

            await WriteText(parts, writer);
            await writer.FlushAsync();
        }

        Console.WriteLine("Wrote system description to file " + input.FileFlag);

        return true;
    }


    public static async Task WriteText(IDescribedSystemPart[] parts, TextWriter writer)
    {
        foreach (var part in parts)
        {
            await writer.WriteLineAsync("## " + part.Title);
            await writer.WriteLineAsync();
            await part.Write(writer);
            await writer.WriteLineAsync();
            await writer.WriteLineAsync();
        }
    }

    public static async Task WriteToConsole(IDescribedSystemPart[] parts)
    {
        foreach (var part in parts)
        {
            var rule = new Rule($"[blue]{part.Title}[/]")
            {
                Justification = Justify.Left
            };

            AnsiConsole.Write(rule);

            if (part is IWriteToConsole o)
            {
                await o.WriteToConsole();
            }
            else
            {
                await part.Write(Console.Out);
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}

#region sample_AboutThisAppPart

public class AboutThisAppPart : IDescribedSystemPart
{
    private readonly IHostEnvironment _host;

    public AboutThisAppPart(IHostEnvironment host, IConfiguration configuration)
    {
        _host = host;
        Title = "About " + Assembly.GetEntryAssembly()?.GetName().Name ?? "This Application";
    }

    public string Title { get; }

    public Task Write(TextWriter writer)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        writer.WriteLine($"          Entry Assembly: {entryAssembly.GetName().Name}");
        writer.WriteLine($"                 Version: {entryAssembly.GetName().Version}");
        writer.WriteLine($"        Application Name: {_host.ApplicationName}");
        writer.WriteLine($"             Environment: {_host.EnvironmentName}");
        writer.WriteLine($"       Content Root Path: {_host.ContentRootPath}");
        writer.WriteLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");

        return Task.CompletedTask;
    }
}

#endregion

#region sample_ReferencedAssemblies

public class ReferencedAssemblies : IDescribedSystemPart, IWriteToConsole
{
    public string Title { get; } = "Referenced Assemblies";

    // If you're writing to a file, this method will be called to 
    // write out markdown formatted text
    public Task Write(TextWriter writer)
    {
        var referenced = Assembly.GetEntryAssembly().GetReferencedAssemblies();
        foreach (var assemblyName in referenced) writer.WriteLine("* " + assemblyName);

        return Task.CompletedTask;
    }

    // If you're only writing to the console, you can implement the
    // IWriteToConsole method and optionally use Spectre.Console for
    // enhanced displays
    public Task WriteToConsole()
    {
        var table = new Table();
        table.AddColumn("Assembly Name");
        table.AddColumn("Version");

        var referenced = Assembly.GetEntryAssembly().GetReferencedAssemblies();
        foreach (var assemblyName in referenced) table.AddRow(assemblyName.Name, assemblyName.Version.ToString());

        AnsiConsole.Write(table);

        return Task.CompletedTask;
    }
}

#endregion