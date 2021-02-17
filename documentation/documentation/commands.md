<!--Building Commands-->

<[info]>
It is perfectly legal to use the same input class across multiple commands
<[/info]>

Oakton commands consist of two parts:

1. A concrete input class that holds all the argument and flag data inputs
1. A concrete class that inherits from `OaktonCommand<T>` or `OaktonAsyncCommand<T>` where the "T" is the input class in the first bullet point

Looking again at the `NameCommand` from the <[linkto:documentation/getting_started]> topic:

<[sample:NameCommand]>

There's only a couple things to note about a command class:

* The only entry point is the `Execute()` method
* The boolean return from the `Execute()` method simply denotes whether or not the command completed successfully. This
  will be important for any kind of console application that you'll want to use in automated builds to prevent false positive
  results
* The `Usages` syntax in the constructor is explained in a section below
* The `[Description]` attribute on the class is strictly for the purpose of providing help text and is not mandatory

If you want to make use of `async/await`, you can inherit from `OaktonAsyncCommand<T>` instead.  The only difference is signature of the `Execute()` method:

<[sample:async-command]>


## Argument Usages

As shown in the `NameCommand` in the section above, you can specify the valid combinations of arguments and the order
in which they should follow in the command line usage by modifying the `Usages` property in the constructor function
of a command:

<[sample:specifying-usages]>

If you do not explicitly specify usages, Oakton will assume that all arguments are mandatory and in the order in which
they appear within the input class.


## Specifying Command Names

By default, Oakton determines the command name for a command class by taking the class name, removing the "Command" suffix, and then using the all lower case remainder of the string. For an example, a command class called `CleanCommand` would have the command name
*clean*. To override that behavior, you can use the `Alias` property on Oakton's `[Description]` attribute as shown below:

<[sample:command-alias]>

## Asynchronous Commands

Oakton also supports the ability to write asynchronous commands that take advantage of
the ability to use asynchronous `Program.Main()` method signatures in recent versions of .Net.

To write an asynchronous command, use the `OaktonAsyncCommand<T>` type as your base class for your
command as shown below:

<[sample:async-command-sample]>

Likewise, to execute asynchronously from `Program.Main()`, there are new overloads on 
`CommandExecutor` for async:

<[sample:MultipleCommands.Program.Main.Async]>


