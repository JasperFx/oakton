<!--title:Bootstrapping with CommandExecutor-->

The easiest way to bootstrap Oakton is to use the <[linkto:documentation/hostbuilder;title=integration with HostBuilder]>. Eschewing that, you have the options in this page.

Oakton applications can be bootstrapped either very simply with a single command, or more elaborately with 
options to preprocess commands, automatic command discovery, <[linkto:documentation/opts;title=options files]>, or custom
command object builders.

## Single Command

If all you have is a single command in your project, the bootstrapping can be as simple as this:

<!-- snippet: sample_Quickstart.Program1 -->
<a id='snippet-sample_quickstart.program1'></a>
```cs
class Program
{
    static int Main(string[] args)
    {
        // As long as this doesn't blow up, we're good to go
        return CommandExecutor.ExecuteCommand<NameCommand>(args);
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L8-L17' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_quickstart.program1' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Multiple Commands

For more complex applications with multiple commands, you need to interact a little more with the `CommandFactory`
configuration as shown below:

<!-- snippet: sample_bootstrapping_command_executor -->
<a id='snippet-sample_bootstrapping_command_executor'></a>
```cs
public static int Main(string[] args)
{
    var executor = CommandExecutor.For(_ =>
    {
        // Automatically discover and register
        // all OaktonCommand's in this assembly
        _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
        
        // You can also add commands explicitly from
        // any assembly
        _.RegisterCommand<HelloCommand>();
        
        // In the absence of a recognized command name,
        // this is the default command to try to 
        // fit to the arguments provided
        _.DefaultCommand = typeof(ColorCommand);

        
        _.ConfigureRun = run =>
        {
            // you can use this to alter the values
            // of the inputs or actual command objects
            // just before the command is executed
        };
        
        // This is strictly for the as yet undocumented
        // feature in stdocs to generate and embed usage information
        // about console tools built with Oakton into
        // stdocs generated documentation websites
        _.SetAppName("MyApp");
    });

    // See the page on Opts files
    executor.OptionsFile = "myapp.opts";

    return executor.Execute(args);
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/OaktonSample/Program.cs#L10-L48' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_bootstrapping_command_executor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Note the usage of `ConfigureRun`. See the [Marten.CommandLine](https://github.com/JasperFx/marten/blob/master/src/Marten.CommandLine/MartenCommands.cs#L16-L21) usage of this extension point as an example.

## Custom Command Creators

::: tip warning
Oakton was purposely built without direct support for an IoC container so users could
focus on building fast console tools without the extra complexity of IoC set up
:::

By default, Oakton just tries to create command objects by calling an expected default, no arg constructor
with `Activator.CreateInstance()`. However, if you want to do something different like use an IoC container, you
can provide a custom `ICommandCreator` like this one using [StructureMap](http://structuremap.github.io):

<!-- snippet: sample_StructureMapCommandCreator -->
<a id='snippet-sample_structuremapcommandcreator'></a>
```cs
public class StructureMapCommandCreator : ICommandCreator
{
    private readonly IContainer _container;

    public StructureMapCommandCreator(IContainer container)
    {
        _container = container;
    }

    public IOaktonCommand CreateCommand(Type commandType)
    {
        return (IOaktonCommand)_container.GetInstance(commandType);
    }

    public object CreateModel(Type modelType)
    {
        return _container.GetInstance(modelType);
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/OaktonSample/Program.cs#L63-L83' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_structuremapcommandcreator' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To use this custom command creator, just tell `CommandExecutor` about it like this:

<!-- snippet: sample_bootstrapping_with_custom_command_factory -->
<a id='snippet-sample_bootstrapping_with_custom_command_factory'></a>
```cs
public static void Bootstrapping(IContainer container)
{
    var executor = CommandExecutor.For(_ =>
    {
        // do the other configuration of the CommandFactory
    }, new StructureMapCommandCreator(container));
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/OaktonSample/Program.cs#L51-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_bootstrapping_with_custom_command_factory' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


