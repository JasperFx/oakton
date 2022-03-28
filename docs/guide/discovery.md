# Command Assembly Discovery

This feature probably won't be commonly used, but there is a mechanism to automatically find and load Oakton commands from other assemblies through file scanning.

The first step is to mark any assembly containing Oakton commands you want discovered and loaded through this mechanism with this attribute:

<!-- snippet: sample_using_OaktonCommandAssemblyAttribute -->
<a id='snippet-sample_using_oaktoncommandassemblyattribute'></a>
```cs
[assembly:Oakton.OaktonCommandAssembly]
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/AspNetCoreExtensionCommands/BuildCommand.cs#L4-L6' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_using_oaktoncommandassemblyattribute' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Next, when you build a `CommandFactory`, you need to explicitly opt into the auto-discovery of commands by using the `RegisterCommandsFromExtensionAssemblies()` option as shown below in the Oakton.AspNetCore code:

<!-- snippet: sample_using_extension_assemblies -->
<a id='snippet-sample_using_extension_assemblies'></a>
```cs
return CommandExecutor.For(factory =>
{
    factory.RegisterCommands(typeof(RunCommand).GetTypeInfo().Assembly);
    if (applicationAssembly != null) factory.RegisterCommands(applicationAssembly);

    // This method will direct the CommandFactory to go look for extension
    // assemblies with Oakton commands
    factory.RegisterCommandsFromExtensionAssemblies();

    factory.ConfigureRun = cmd =>
    {
        if (cmd.Input is IHostBuilderInput i) i.HostBuilder = source;
    };
});
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/Oakton/CommandLineHostingExtensions.cs#L101-L116' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_using_extension_assemblies' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
