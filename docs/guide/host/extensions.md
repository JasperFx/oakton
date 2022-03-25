<!--title:Writing Extension Commands-->

Oakton has a strong extensibility model to find and activate commands from external assemblies. If an application uses the `RunOaktonCommands(args)` method, Oakton will look for any Oakton commands in any assembly that has this assembly level attribute:

<!-- snippet: sample_using_OaktonCommandAssemblyAttribute -->
<a id='snippet-sample_using_oaktoncommandassemblyattribute'></a>
```cs
[assembly:Oakton.OaktonCommandAssembly]
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/AspNetCoreExtensionCommands/BuildCommand.cs#L4-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_using_oaktoncommandassemblyattribute' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

::: tip warning
You will have to explicitly add this attribute to the main assembly of your application to make Oakton discover commands in that assembly. Oakton no longer supports trying to walk the call stack to determine the main application assembly.
:::

Extension commands can be either basic `OaktonCommand` or `OaktonAsyncCommand` classes. To add an extension command that uses the `HostBuilder` configuration of the application, the command needs to use the `NetCoreInput` class or a class that inherits from `NetCoreInput`. In this simple example below, I've built a command that just tries to do a "smoke test" by calling the `HostBuilder.Build()` method and seeing if any exceptions happen:

<!-- snippet: sample_SmokeCommand -->
<a id='snippet-sample_smokecommand'></a>
```cs
[Description("Simply try to build a web host as a smoke test", Name = "smoke")]
public class SmokeCommand : OaktonCommand<NetCoreInput>
{
    public override bool Execute(NetCoreInput input)
    {
        // This method builds out the IWebHost for your
        // configured IHostBuilder of the application
        using (var host = input.BuildHost())
        {
            Console.WriteLine("It's all good");
        }

        return true;
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/AspNetCoreExtensionCommands/BuildCommand.cs#L11-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_smokecommand' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

The `NetCoreInput` carries the `IHostBuilder` of your application, but does **not** start up or build the `IHost` by itself. You would have to explicitly do so, but making that lazy gives you the ability to alter or extend the application configuration before calling `IHostBuilder.Build()` or `IHostBuilder.Start()`.
