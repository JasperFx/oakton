<!--title:Parsing Arguments and Optional Flags-->

## Arguments

An argument is any public field or settable property on the input class that is not suffixed with "Flag" or marked
with the `[IgnoreOnCommandLine]`. Below is a sample:

<!-- snippet: sample_sample_arguments -->
<a id='snippet-sample_sample_arguments'></a>
```cs
public class NameInput
{
    // Arguments can be settable properties
    public string First { get; set; }
    
    // Arguments can be public fields
    public string Last;

    // Read only properties are ignored
    public string Fullname => $"{First} {Last}";
    
    // You can explicitly ignore public fields or 
    // properties that should not be captured at
    // the command line
    [IgnoreOnCommandLine]
    public string Nickname;
    
    // This would be considered to be a flag,
    // not an argument
    public bool VerboseFlag { get; set; }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/OaktonSample/Program.cs#L138-L160' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_sample_arguments' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Valid Argument Types

Argument values can be of type:

* String -- wrap a string with spaces in quotes like `"Kareem Abdul Jabbar"`
* Number -- but you'll need to wrap data in parantheses for negative numbers like `"-1"` so Oakton doesn't treat it like a flag
* Enumeration
* Guid
* DateTime -- in [Iso8601](https://en.wikipedia.org/wiki/ISO_8601) format, or it falls back to `DateTime.Parse()`. Relative dates can be expressed like `TODAY` or `TODAY-3` or `TODAY+3` for today, 3 days ago, and 3 days from now respectively
* Any concrete type that has a constructor with a single string argument like `new Dimensions("2 x 4")`
* Arrays of any of the types above in comma delimited form, but you would have to wrap the logical array values in parentheses like `"1,2,3,4"`
* TimeSpan -- express as a time in 24 hour time like `1200` or `12:30`. See the [unit tests for TimeSpan conversions](https://github.com/JasperFx/baseline/blob/master/src/Baseline.Testing/Conversion/TimeSpanConverterTester.cs) for more examples.

The conversions are done via the [Baseline](https://github.com/JasperFx/baseline) library.

## Array or Enumerable Arguments

You *can* use array or enumerable arguments as shown on this input type:

<!-- snippet: sample_EnumerableArguments -->
<a id='snippet-sample_enumerablearguments'></a>
```cs
public class EnumerableArgumentInput
{
    public IEnumerable<string> Names { get; set; }

    public IEnumerable<string> OptionalFlag { get; set; }
        
    public IEnumerable<TargetEnum> Enums { get; set; }

    [Description("ages of target")]
    public IEnumerable<int> Ages { get; set; }

}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/Tests/EnumerableArgumentTester.cs#L63-L76' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_enumerablearguments' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There's one huge caveat though, **an enumerable or array variable will need to be the very last argument** in your command line usage.

The usage would be:

```
executable command value1 value2 value3 --flag --flag2
```

## Valid Argument Patterns

Look at the input class from the <[linkto:documentation/getting_started]> tutorial:

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
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/quickstart/Program.cs#L19-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_nameinput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In this particular case, I want users to be able to enter either the `Name` argument or both the `Name` and
`Color` arguments. In the matching command class for this input, I specify two `Usages` patterns for this command:

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

The call to `Usage` in the constructor function specifies the valid argument patterns for the command. If that explicit
`Usage` configuration was omitted, Oakton assumes that every argument is always mandatory and appears in the order of the
properties or fields in the input class.

## Optional Flags

Flags are any public settable property or public field on the input class that are suffixed with 
*Flag*. Below is an example of several flags:

<!-- snippet: sample_CleanInput -->
<a id='snippet-sample_cleaninput'></a>
```cs
public class CleanInput
{
    [Description("Do it now!")]
    public bool ForceFlag { get; set; }
    
    [FlagAlias('d')]
    [Description("Remove untracked directories in addition to untracked files")]
    public bool RemoveUntrackedDirectoriesFlag { get; set; }
    
    [FlagAlias('x')]
    [Description("Remove only files ignored by Git")]
    public bool DoNoUseStandardIgnoreRulesFlag { get; set; }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MultipleCommands/Program.cs#L76-L90' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_cleaninput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Setting Flag Values

Let's say you're trying to recreate the command line options for the `git checkout` command with this input:

<!-- snippet: sample_CheckoutInput -->
<a id='snippet-sample_checkoutinput'></a>
```cs
public class CheckoutInput
{
    [FlagAlias("create-branch",'b')]
    public string CreateBranchFlag { get; set; }
    
    public bool DetachFlag { get; set; }
    
    public bool ForceFlag { get; set; }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MultipleCommands/Program.cs#L63-L73' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_checkoutinput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

To specify the name of a newly created branch using the `CreateBranchFlag` property, I can either use:

```
git checkout -b working
```

to create a new "working" branch, or use the long form:

```
git checkout --create-branch working
```

When Oakton encounters a recognized flag that accepts a value, it assumes that the next string value should be converted
to that named value and bound to the proper field or property.

## Flag Alias

Oakton follows the Unix idiom of command line flags being identified as either two dashes and a word for
the long form, or by a single dash and a letter as a shorthand. By default, a flag in the command line is derived from the property or field name by omitting the *Flag* suffix for the long form and taking the first letter for the short hand.

By that logic, the property named `ForceFlag` from the example above would be used like this (assuming that the executable is named 'git'):

```
git clean --force
```

or

```
git clean -f
```

There is some logic to deal with bigger property names by splitting based on [Pascal casing rules](http://wiki.c2.com/?PascalCase). So
a property named `FirstNameFlag` would be parsed at the command line as either `-f` or `--first-name-flag`.

You can of course override the flag alias in either long or short form by using the `[FlagAlias]` attribute as shown in the `CleanInput`
example above.

Lastly, if only the long form alias is desired, `[FlagAlias]` provides the constructor `FlagAliasAttribute(string longAlias, bool longAliasOnly)`.

Just like Arguments, Flags can be any type that Oakton knows how to convert, with a few special types shown in the subsequent sections.

## Boolean Flags

Boolean flags are just a little bit different because there's no value necessary. If the flag appears in the command line arguments,
the field or property is set to `true`.

Consider our recreation of the `git clean` command:

<!-- snippet: sample_CleanInput -->
<a id='snippet-sample_cleaninput'></a>
```cs
public class CleanInput
{
    [Description("Do it now!")]
    public bool ForceFlag { get; set; }
    
    [FlagAlias('d')]
    [Description("Remove untracked directories in addition to untracked files")]
    public bool RemoveUntrackedDirectoriesFlag { get; set; }
    
    [FlagAlias('x')]
    [Description("Remove only files ignored by Git")]
    public bool DoNoUseStandardIgnoreRulesFlag { get; set; }
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MultipleCommands/Program.cs#L76-L90' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_cleaninput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

From the command line, I can use these boolean flags like this with the long form for `ForceFlag`:

```
git clean -x -d --force 
```

or with all short names:

```
git clean -x -d --f
```

or using the Unix idiom of being able to combine flags in one expression like this ("git clean -xfd" is what I use myself):

```
git clean -xfd
```

All of the usages above are exact equivalents.

## Enumerable Flags

You can use enumerable types like `string[]` or `IEnumerable<string>` for flag arguments to add multiple values. Flags are a little
more forgiving in this usage than arguments because Oakton can rely on the start of another flag to "know" when we've finished collecting
values for that array.

Let's say we have this input:

<!-- snippet: sample_FileInput -->
<a id='snippet-sample_fileinput'></a>
```cs
public class FileInput
{
    public string[] FilesFlag;

    public string[] DirectoriesFlag;
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/Tests/FlagTester.cs#L160-L167' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_fileinput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In usage, the flags could be used like:

```
executable command --files one.txt two.txt "c:\folder\file.txt" --directories c:\folder1 c:\folder2
```

## Key/Value Flags


::: tip warning
Key/Value flags have to be of type `Dictionary<string, string>` or `IDictionary<string, string>`
:::

New to Oakton 1.3 is the ability to **finally** express key/value pairs as a special kind of flag. Let's say that we want
to capture extensible key/value pairs on our input class like this:

<!-- snippet: sample_DictInput -->
<a id='snippet-sample_dictinput'></a>
```cs
public class DictInput
{
    public Dictionary<string, string> PropFlag = new Dictionary<string, string>();
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/Tests/DictionaryFlagTester.cs#L45-L50' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_dictinput' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

In usage at the command line, the flag is used like this:

```
executable command --prop:color Red --prop:direction North
```

When this command line is parsed, the `PropFlag` property above will be a dictionary with the values
`color=Red` and `direction=North`.
