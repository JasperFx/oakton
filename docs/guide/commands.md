<!--Building Commands-->

::: tip
It is perfectly legal to use the same input class across multiple commands
:::

Oakton commands consist of two parts:

1. A concrete input class that holds all the argument and flag data inputs
1. A concrete class that inherits from `OaktonCommand<T>` or `OaktonAsyncCommand<T>` where the "T" is the input class in the first bullet point

Looking again at the `NameCommand` from the [getting started](/guide) topic:

<!-- snippet: sample_NameCommand -->
<a id='snippet-sample_namecommand'></a>
```cs
[Description("Print somebody's name")]
public class NameCommand : OaktonCommand<NameInput>
{
    public NameCommand()
    {
        // The usage pattern definition here is completely
        // optional
        Usage("Default Color").Arguments(x => x.Name);
        Usage("Print name with specified color").Arguments(x => x.Name, x => x.Color);
    }

    public override bool Execute(NameInput input)
    {
        var text = input.Name;
        if (!string.IsNullOrEmpty(input.TitleFlag))
        {
            text = input.TitleFlag + " " + text;
        }

        // This is a little helper in Oakton for getting
        // cute with colors in the console output
        ConsoleWriter.Write(input.Color, text);

        // Just telling the OS that the command
        // finished up okay
        return true;
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L33-L63' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_namecommand' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There's only a couple things to note about a command class:

* The only entry point is the `Execute()` method
* The boolean return from the `Execute()` method simply denotes whether or not the command completed successfully. This
  will be important for any kind of console application that you'll want to use in automated builds to prevent false positive
  results
* The `Usages` syntax in the constructor is explained in a section below
* The `[Description]` attribute on the class is strictly for the purpose of providing help text and is not mandatory

If you want to make use of `async/await`, you can inherit from `OaktonAsyncCommand<T>` instead.  The only difference is signature of the `Execute()` method:

<!-- snippet: sample_async_command -->
<a id='snippet-sample_async_command'></a>
```cs
public class DoNameThingsCommand : OaktonAsyncCommand<NameInput>
{
    public override async Task<bool> Execute(NameInput input)
    {
        ConsoleWriter.Write(input.Color, "Starting...");
        await Task.Delay(TimeSpan.FromSeconds(3));

        ConsoleWriter.Write(input.Color, $"Done! Hello {input.Name}");
        return true;
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L101-L113' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_async_command' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Argument Usages

As shown in the `NameCommand` in the section above, you can specify the valid combinations of arguments and the order
in which they should follow in the command line usage by modifying the `Usages` property in the constructor function
of a command:

<!-- snippet: sample_specifying_usages -->
<a id='snippet-sample_specifying_usages'></a>
```cs
public OtherNameCommand()
{
    // You can specify multiple usages
    Usage("describe what is different about this usage")
        // Specify which arguments are part of this usage
        // and in what order they should be expressed
        // by the user
        .Arguments(x => x.Name, x => x.Color)

        // Optionally, you can provide a white list of valid
        // flags in this usage
        .ValidFlags(x => x.TitleFlag);
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L68-L82' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_specifying_usages' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

If you do not explicitly specify usages, Oakton will assume that all arguments are mandatory and in the order in which
they appear within the input class.


## Specifying Command Names

By default, Oakton determines the command name for a command class by taking the class name, removing the "Command" suffix, and then using the all lower case remainder of the string. For an example, a command class called `CleanCommand` would have the command name
*clean*. To override that behavior, you can use the `Alias` property on Oakton's `[Description]` attribute as shown below:

<!-- snippet: sample_command_alias -->
<a id='snippet-sample_command_alias'></a>
```cs
[Description("Say my name differently", Name = "different-name")]
public class AliasedCommand : OaktonCommand<NameInput>
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L90-L93' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_command_alias' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Asynchronous Commands

Oakton also supports the ability to write asynchronous commands that take advantage of
the ability to use asynchronous `Program.Main()` method signatures in recent versions of .Net.

To write an asynchronous command, use the `OaktonAsyncCommand<T>` type as your base class for your
command as shown below:

<!-- snippet: sample_async_command_sample -->
<a id='snippet-sample_async_command_sample'></a>
```cs
[Description("Say my name", Name = "say-async-name")]
public class AsyncSayNameCommand : OaktonAsyncCommand<SayName>
{
    public AsyncSayNameCommand()
    {
        Usage("Capture the users name").Arguments(x => x.FirstName, x => x.LastName);
    }

    public override async Task<bool> Execute(SayName input)
    {
        await Console.Out.WriteLineAsync($"{input.FirstName} {input.LastName}");

        return true;
    }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/Tests/CommandExecutorTester.cs#L160-L176' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_async_command_sample' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Likewise, to execute asynchronously from `Program.Main()`, there are new overloads on 
`CommandExecutor` for async:

<!-- snippet: sample_MultipleCommands.Program.Main.Async -->
<a id='snippet-sample_multiplecommands.program.main.async'></a>
```cs
static Task<int> Main(string[] args)
{
    var executor = CommandExecutor.For(_ =>
    {
        // Find and apply all command classes discovered
        // in this assembly
        _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
    });

    return executor.ExecuteAsync(args);
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MultipleCommands/Program.cs#L26-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_multiplecommands.program.main.async' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


