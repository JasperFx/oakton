<!--title:The "describe" command-->

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

<[sample:IDescribedSystemPart]>

Or if you have a related group of parts, you can register custom implementations of the `IDescribedSystemPartFactory` as well:

<[sample:IDescribedSystemPartFactory]>

Oakton adds a couple extension methods on `IServiceCollection` to help you register custom describers:

<[sample:extending-describe]>

For an example, here's the implementation for one of the built in described system parts:

<[sample:AboutThisAppPart]>

You can also opt into enhanced formatting in the console output using the [Spectre.Console](https://spectresystems.github.io/spectre.console/) library if your part implements the `IWriteToConsole` interface like this built in part:

<[sample:ReferencedAssemblies]>



