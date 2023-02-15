# The "describe" command

The new `describe` command that comes with Oakton V3.0+ can be used as a generic diagnostic capability to look into the configuration of your .Net application that's bootstrapped with `IHostBuilder`. 

Out of the box, the `describe` command will simply preview some information about your main application assembly, its version, the application's `IHostEnvironment` settings, and the immediately reference assemblies (it's not a recursive query) with output like this:

```
── About EnvironmentCheckDemonstrator ──────────────────────────────────────────
          Entry Assembly: EnvironmentCheckDemonstrator
                 Version: 1.0.0.0
        Application Name: EnvironmentCheckDemonstrator
             Environment: Production
       Content Root Path: /Users/jeremydmiller/code/oakton/src/EnvironmentCheckDemonstrator
AppContext.BaseDirectory: /Users/jeremydmiller/code/oakton/src/EnvironmentCheckDemonstrator/bin/Debug/net5.0/


── Referenced Assemblies ────────────────────────────────────────────────────────
┌───────────────────────────────────────────────────────┬─────────┐
│ Assembly Name                                         │ Version │
├───────────────────────────────────────────────────────┼─────────┤
│ System.Runtime                                        │ 5.0.0.0 │
│ Oakton                                                │ 3.0.0.0 │
│ Microsoft.Extensions.DependencyInjection.Abstractions │ 5.0.0.0 │
│ System.ComponentModel                                 │ 5.0.0.0 │
│ System.Console                                        │ 5.0.0.0 │
│ Microsoft.Extensions.Hosting                          │ 5.0.0.0 │
│ Microsoft.Extensions.Hosting.Abstractions             │ 5.0.0.0 │
│ Baseline                                              │ 2.1.1.0 │
└───────────────────────────────────────────────────────┴─────────┘

```


The command line flags are shown below:

```
                  [-f, --file <file>] -> Optionally write the description to the given file location
                       [-s, --silent] -> Do not write any output to the console
                [-t, --title <title>] -> Filter the output to only a single described part
                         [-l, --list] -> If set, the command only lists the known part titles
                  [-i, --interactive] -> If set, interactively select which part(s) to preview
    [-e, --environment <environment>] -> Use to override the ASP.Net Environment name
                      [-v, --verbose] -> Write out much more information at startup and enables console logging
         [-l, --log-level <loglevel>] -> Override the log level
          [----config:<prop> <value>] -> Overwrite individual configuration items

```

As you can hopefull tell, the `describe` command can be used to preview the diagnostic information in the console and optionally write the descriptive text to a file like this, where `myapp.md` is the file name you want the output written to:

```
dotnet run -- describe --file myapp.md
```

If you have many described parts in your system, you can use the `-i` or `--interactive` flag to interactively select which parts you want to view or export to a file.

## Extending describe

The `describe` command can be extended by registering custom implemtations of the `IDescribedSystemPart` interface in your application container:

<!-- snippet: sample_IDescribedSystemPart -->
<a id='snippet-sample_idescribedsystempart'></a>
```cs
/// <summary>
/// Base class for a "described" part of your application.
/// Implementations of this type should be registered in your
/// system's DI container to be exposed through the "describe"
/// command
/// </summary>
public interface IDescribedSystemPart
{
    /// <summary>
    /// A descriptive title to be shown in the rendered output
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Write markdown formatted text to describe this system part
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    Task Write(TextWriter writer);
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/Oakton/Descriptions/IDescribedSystemPart.cs#L6-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_idescribedsystempart' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Or if you have a related group of parts, you can register custom implementations of the `IDescribedSystemPartFactory` as well:

<!-- snippet: sample_IDescribedSystemPartFactory -->
<a id='snippet-sample_idescribedsystempartfactory'></a>
```cs
/// <summary>
/// Register implementations of this service to help
/// the describe command discover additional system parts
/// </summary>
public interface IDescribedSystemPartFactory
{
    IDescribedSystemPart[] Parts();
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/Oakton/Descriptions/IDescribedSystemPartFactory.cs#L3-L12' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_idescribedsystempartfactory' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Oakton adds a couple extension methods on `IServiceCollection` to help you register custom describers:

<!-- snippet: sample_extending_describe -->
<a id='snippet-sample_extending_describe'></a>
```cs
static Task<int> Main(string[] args)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            
            for (int i = 0; i < 5; i++)
            {
                services.AddSingleton<IEnvironmentCheck>(new GoodEnvironmentCheck(i + 1));
                services.AddSingleton<IEnvironmentCheck>(new BadEnvironmentCheck(i + 1));

                services.AddSingleton<IStatefulResource>(new FakeResource("Database", "Db " + (i + 1)));
            }

            services.AddSingleton<IStatefulResource>(new FakeResource("Bad", "Blows Up")
            {
                Failure = new DivideByZeroException()
            });
            
            services.CheckEnvironment("Inline, async check", async (services, token) =>
            {
                await Task.Delay(1.Milliseconds(), token);

                throw new Exception("I failed!");
            });
            
            
            // This is an example of adding custom
            // IDescriptionSystemPart types to your
            // application that can participate in
            // the describe output
            services.AddDescription<Describer1>();
            services.AddDescription<Describer2>();
            services.AddDescription<Describer3>();

        })
        .RunOaktonCommands(args);
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/EnvironmentCheckDemonstrator/Program.cs#L16-L56' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_extending_describe' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

For an example, here's the implementation for one of the built in described system parts:

<!-- snippet: sample_AboutThisAppPart -->
<a id='snippet-sample_aboutthisapppart'></a>
```cs
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
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/Oakton/Descriptions/DescribeCommand.cs#L128-L153' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_aboutthisapppart' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

You can also opt into enhanced formatting in the console output using the [Spectre.Console](https://spectresystems.github.io/spectre.console/) library if your part implements the `IWriteToConsole` interface like this built in part:

<!-- snippet: sample_ReferencedAssemblies -->
<a id='snippet-sample_referencedassemblies'></a>
```cs
public class ReferencedAssemblies : IDescribedSystemPart, IWriteToConsole
{
    public string Title { get; } = "Referenced Assemblies";
    
    // If you're writing to a file, this method will be called to 
    // write out markdown formatted text
    public Task Write(TextWriter writer)
    {
        var referenced = Assembly.GetEntryAssembly().GetReferencedAssemblies();
        foreach (var assemblyName in referenced)
        {
            writer.WriteLine("* " + assemblyName);
        }

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
        foreach (var assemblyName in referenced)
        {
            table.AddRow(assemblyName.Name, assemblyName.Version.ToString());
        }
        
        AnsiConsole.Write(table);

        return Task.CompletedTask;
    }
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/Oakton/Descriptions/DescribeCommand.cs#L155-L193' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_referencedassemblies' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->



