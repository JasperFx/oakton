<!--title:Writing Extension Commands-->

Oakton has a strong extensibility model to find and activate commands from external assemblies. If an application uses the `RunOaktonCommands(args)` method, Oakton will look for any Oakton commands in any assembly that has this assembly level attribute:

<[sample:using-OaktonCommandAssemblyAttribute]>

<[warning]>
You will have to explicitly add this attribute to the main assembly of your application to make Oakton discover commands in that assembly. Oakton no longer supports trying to walk the call stack to determine the main application assembly.
<[/warning]>

Extension commands can be either basic `OaktonCommand` or `OaktonAsyncCommand` classes. To add an extension command that uses the `HostBuilder` configuration of the application, the command needs to use the `NetCoreInput` class or a class that inherits from `NetCoreInput`. In this simple example below, I've built a command that just tries to do a "smoke test" by calling the `HostBuilder.Build()` method and seeing if any exceptions happen:

<[sample:SmokeCommand]>

The `NetCoreInput` carries the `IHostBuilder` of your application, but does **not** start up or build the `IHost` by itself. You would have to explicitly do so, but making that lazy gives you the ability to alter or extend the application configuration before calling `IHostBuilder.Build()` or `IHostBuilder.Start()`.