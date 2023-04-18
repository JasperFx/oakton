# Help Text

Oakton comes with its own `[Description]` attribute that can be applied to fields or properties on
the input class to provide help information on usage like this:

<!-- snippet: sample_NameInput -->
<a id='snippet-sample_nameinput'></a>
```cs
public class NameInput
{
    [Description("The name to be printed to the console output")]
    public string Name { get; set; }

    [Description("The color of the text. Default is black")]
    public ConsoleColor Color { get; set; } = ConsoleColor.Black;

    [Description("Optional title preceeding the name")]
    public string TitleFlag { get; set; }
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/quickstart/Program.cs#L19-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_nameinput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

or on the command class itself:

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

        AnsiConsole.Write($"[{input.Color}]{text}[/]");

        // Just telling the OS that the command
        // finished up okay
        return true;
    }
}
```
<sup><a href='https://github.com/JasperFx/oakton/blob/master/src/quickstart/Program.cs#L33-L60' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_namecommand' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Also note the explanatory text in the `Usage()` method above in the case of a command that has multiple valid
argument patterns.

To display a list of all the available commands, you can type either:

```
executable help
```

or 

```
executable ?
```

Likewise, to get the usage help for a single command named "clean", use either `executable help clean` or `executable ? clean`.
