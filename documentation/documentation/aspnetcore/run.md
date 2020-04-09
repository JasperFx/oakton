<!--Improved "Run" Command-->

To run your application normally from a command prompt with all the default configuration **from the project root directory**, there's no real change from what you'd do without *Oakton.AspNetCore*. The command is still just:

```
dotnet run
```

If your command prompt is at the solution directory (my personal default), you can use all the available [dotnet run](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21) options, and in 
this case tell the dotnet CLI to run a project in another directory like this example:

```
dotnet run --project src/MvcApp/MvcApp.csproj
```

So far, no changes from what you have today, so let's dig into what's changed. First, at any time to see the list of
available commands in your system, use either the command `dotnet run -- ?` or `dotnet run -- help` as shown below:


```
dotnet run -- ?
```

Which gave this output on a sample MVC application with commands from an extension library:

```
Searching 'AspNetCoreExtensionCommands, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' for commands

  -----------------------------------------------------------------------
    Available commands:
  -----------------------------------------------------------------------
    check-env -> Execute all environment checks against the application
          run -> Runs the configured AspNetCore application
        smoke -> Simply try to build a web host as a smoke test
  ------------------------------------------------------------------------
```

<[info]>
When you're using the `dotnet run` command, the usage of the double dashes *--* separates command line arguments to the `dotnet run` command itself from the command arguments to your application. The `args` array passed into your application will be any arguments or flags to the right of the *--* separator.
<[/info]>

The *run* command shown above is the default command for Oakton and what will be executed unless you explicitly choose another named command.

Looking farther into what the *run* command provides with:

```
dotnet run -- ? run
```

gives you this help text for the options on the *run* command:

```
 Usages for 'run' (Runs the configured AspNetCore application)
  run [-c, --check] [-e, --environment <environment>] [-v, --verbose] [-l, --log-level <logleve>] [----config:<prop> <value>]

  ---------------------------------------------------------------------------------------------------------------------------------------
    Flags
  ---------------------------------------------------------------------------------------------------------------------------------------
                        [-c, --check] -> Run the environment checks before starting the host
    [-e, --environment <environment>] -> Use to override the ASP.Net Environment name
                      [-v, --verbose] -> Write out much more information at startup and enables console logging
          [-l, --log-level <logleve>] -> Override the log level
          [----config:<prop> <value>] -> Overwrite individual configuration items
  ---------------------------------------------------------------------------------------------------------------------------------------
```

## Overriding the Hosting Environment

To override the hosting environment that your ASP.Net Core application runs under, use the *environment* flag as shown below:

```
dotnet run -- --environment Testing
```

This would be the equivalent of running your application with this code (note the usage of `UseEnvironment("Testing")`):

```
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CreateWebHostBuilder(args)
                .RunOaktonCommands(args);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()

                // This is what the --environment flag does
                .UseEnvironment("Testing");
        
    }
```

See [Andrew Lock on how the hosting environment can be useful](https://andrewlock.net/how-to-use-multiple-hosting-environments-on-the-same-machine-in-asp-net-core/).


## Overriding Configuration Items

Individual values in your system's [IConfiguration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2) can be overridden at the command line using the *--config* flag like so:

```
dotnet run -- --config:key1 value1 --config:key2 value2
```

The flag usage above would override the system configuration with the values *key1=value1* and *key2=value2*.

## Overriding the Minimum Log Level

You can override the minimum log level in the running application using any valid value of the [LogLevel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-2.2) enumeration and the *--log-level* flag as shown below:

```
dotnet run -- --log-level Information
```

See [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2) for more information about ASP.Net Core logging.


## Running Environment Checks Before Starting the Application

You can also opt into running any configured <[linkto:documentation/aspnetcore/environment;title=environment checks]> before starting Kestrel. If any of the environment checks fail, the application startup will fail. The goal of this feature is to make deployments be self-diagnosing and fail fast at startup time if the system can detect problems in its configuration or with its dependencies.

To run the environment checks as part of the run command, just use the *--environment* flag like this:

```
dotnet run -- --environment
```