<!--title:Bootstrapping-->

Oakton applications can be bootstrapped either very simply with a single command, or more elaborately with 
options to preprocess commands, automatic command discovery, <[linkto:documentation/opts;title=options files]>, or custom
command object builders.

## Single Command

If all you have is a single command in your project, the bootstrapping can be as simple as this:

<[sample:Quickstart.Program1]>

## Multiple Commands

For more complex applications with multiple commands, you need to interact a little more with the `CommandFactory`
configuration as shown below:

<[sample:bootstrapping-command-executor]>

Note the usage of `ConfigureRun`. See the [Marten.CommandLine](https://github.com/JasperFx/marten/blob/master/src/Marten.CommandLine/MartenCommands.cs#L16-L21) usage of this extension point as an example.

## Custom Command Creators

<[info]>
Oakton was purposely built without direct support for an IoC container so users could
focus on building fast console tools without the extra complexity of IoC set up
<[/info]>

By default, Oakton just tries to create command objects by calling an expected default, no arg constructor
with `Activator.CreateInstance()`. However, if you want to do something different like use an IoC container, you
can provide a custom `ICommandCreator` like this one using [StructureMap](http://structuremap.github.io):

<[sample:StructureMapCommandCreator]>

To use this custom command creator, just tell `CommandExecutor` about it like this:

<[sample:bootstrapping-with-custom-command-factory]>


