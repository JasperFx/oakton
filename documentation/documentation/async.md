<!--title:Asynchronous Execution-->

New in Oakton 1.4 is the ability to write asynchronous commands and also to take advantage of
the ability to use asynchronous `Program.Main()` method signatures in recent versions of .Net.

To write an asynchronous command, use the `OaktonAsyncCommand<T>` type as your base class for your
command as shown below:

<[sample:async-command-sample]>

Likewise, to execute asynchronously from `Program.Main()`, there are new overloads on 
`CommandExecutor` for async:

<[sample:MultipleCommands.Program.Main.Async]>

