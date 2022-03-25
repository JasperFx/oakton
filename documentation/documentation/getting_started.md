<!--title: Getting Started-->

::: tip warning
The power of Oakton really comes into play when it's combined with applications using the `HostBuilder`
mechanism for bootstrapping .Net applications. See <[linkto:documentation/hostbuilder]> for more information.
:::

Oakton originated in the now defunct [FubuCore](https://github.com/DarthFubuMVC/fubucore) project in 2010 as a mechanism to allow our .Net
development team of that time to build robust console line utilities with these attributes:

* Make it easy to embed and expose command help for user friendliness
* Be consistent with Unix/Linux command line idioms from arguments and optional names -- i.e., make the command line usage
  feel like Git's command line syntax
* Easily enable tools to expose multiple commands
* Completely separate the command line parsing from the actual functionality of the console application for easier testing of the command line tools and cleaner code
* Validate user input and helpfully tell them when it's invalid and what the correct usage should be

Oakton was originally extracted from FubuCore as a new standalone project before adding some new improvements and support for .Net Core and now .Net 5.0.

## Your First Command

To get started, simply create a new dotnet console application and add the `Oakton` nuget dependency. For your first command, let's start simple with a command that will simply print out a specified name with an optional color and title. The logical first step is to just
create the input class for your command that will establish the arguments and an optional flag:

snippet: sample_NameInput

You'll note that I've added some `[Oakton.Description]` attributes strictly for the purpose of adding user help messages that we'll take a look at later. Now that we've got that out of the way, let's create our first command:

snippet: sample_NameCommand

With that in place, let's wire it up to our applications `Program.Main()`:

snippet: sample_Quickstart.Program1

Now, from the command line, I'll just try `dotnet run`, which will give us this (slightly elided) output complaining
that we're missing a required argument or two:

```
Invalid usage

 Usages for 'name' (Print somebody's name)

  ----------------------------------------------------------------------------------------
    Usages
  ----------------------------------------------------------------------------------------
                      Default Color ->  name <name> [-t, --title <title>]
    Print name with specified color ->  name <name> Black|DarkBlue|DarkGreen|DarkCyan|DarkRed|DarkMagenta|DarkYellow|Gray|DarkGray|Blue|Green|Cyan|Red|Magenta|Yellow|White [-t, --title <title>]
  ----------------------------------------------------------------------------------------

  ----------------------------------------------------------------------------------------
    Arguments
  ----------------------------------------------------------------------------------------
     name -> The name to be printed to the console output
    color -> The color of the text. Default is black
  ----------------------------------------------------------------------------------------

  ----------------------------------------------------------------------------------------
    Flags
  ----------------------------------------------------------------------------------------
    [-t, --title <title>] -> Optional title preceeding the name
  ----------------------------------------------------------------------------------------
```

Do note that we could get the same output without the angry "Invalid Usage" message by:

```
dotnet run -- help
```

or

```
dotnet run -- ?
```

Now, to actually run the command, try:

```
dotnet run -- "Alex Smith"
```

Which will simply print out *Alex Smith* to the console output. We had to wrap the text in quotations so that Oakton treated
the full name with spaces as a single argument.

::: tip warning
The "--" argument shown in the samples here is only a construct of using the "dotnet run" command to separate arguments that should
apply to the dotnet tool on the left from arguments that should be passed into our application. If you run the compiled application
itself, you will not need the "--" separator.
:::

To print the name in blue, we can type `dotnet run -- "Alex Smith" Blue`. To add a title, we can use the signature `dotnet run -- "Alex Smith" --title Mr` or `dotnet run -- "Alex Smith" --t Mr` to get the output "Mr Alex Smith."

## How it Works

Oakton is attempting to take the tokenized values in the command line input and apply the values to the matching fields or properties
on the input class objects before passing that object into the proper command's `Execute()` method. Oakton also validates that the
signature and flag usage matches the known command syntax.

## Multiple Commands in One Tool

Now, let's move on to building a tool with multiple commands. Let's say that we're trying to partially recreate
the git command line with the `clean` and `checkout` commands:

snippet: sample_git_commands

In the `Program.Main()`, the setup is just a little bit different to go discover all the commands held in the application:

snippet: sample_MultipleCommands.Program.Main

Now then, typing `dotnet run` without specifying a valid command will give you this:

```
  ------------------------------------------------------------------
    Available commands:
  ------------------------------------------------------------------
    checkout -> Switch branches or restore working tree files
       clean -> Remove untracked files from the working tree
  ------------------------------------------------------------------
```

Typing `dotnet run -- help` or `dotnet run -- ?` would give you the same information. 

Our fake `clean` command has this usage:

snippet: sample_CleanInput

To see the specific usage of the `clean` command, try `dotnet run -- help clean` or `dotnet run -- ? clean` to get the usage:

```
 Usages for 'clean' (Remove untracked files from the working tree)
  clean [-f, --force] [-d, --remove-untracked-directories] [-x, --do-no-use-standard-ignore-rules]

  -------------------------------------------------------------------------------------------
    Flags
  -------------------------------------------------------------------------------------------
                              [-f, --force] -> Do it now!
       [-d, --remove-untracked-directories] -> Remove untracked directories in addition to untracked files
    [-x, --do-no-use-standard-ignore-rules] -> Remove only files ignored by Git
  -------------------------------------------------------------------------------------------
```

From here, you probably want to learn more about:

* <[linkto:documentation/commands;title=All about commands]>
* <[linkto:documentation/arguments;title=Using arguments and argument signatures]>
* <[linkto:documentation/flags]>
* <[linkto:documentation/help;title=How the help output works]>
* <[linkto:documentation/opts]>
