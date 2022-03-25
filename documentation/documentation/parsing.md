<!--title:Parsing Arguments and Optional Flags-->

## Arguments

An argument is any public field or settable property on the input class that is not suffixed with "Flag" or marked
with the `[IgnoreOnCommandLine]`. Below is a sample:

snippet: sample_sample_arguments

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

snippet: sample_EnumerableArguments

There's one huge caveat though, **an enumerable or array variable will need to be the very last argument** in your command line usage.

The usage would be:

```
executable command value1 value2 value3 --flag --flag2
```

## Valid Argument Patterns

Look at the input class from the <[linkto:documentation/getting_started]> tutorial:

snippet: sample_NameInput

In this particular case, I want users to be able to enter either the `Name` argument or both the `Name` and
`Color` arguments. In the matching command class for this input, I specify two `Usages` patterns for this command:

snippet: sample_NameCommand

The call to `Usage` in the constructor function specifies the valid argument patterns for the command. If that explicit
`Usage` configuration was omitted, Oakton assumes that every argument is always mandatory and appears in the order of the
properties or fields in the input class.

## Optional Flags

Flags are any public settable property or public field on the input class that are suffixed with 
*Flag*. Below is an example of several flags:

snippet: sample_CleanInput

## Setting Flag Values

Let's say you're trying to recreate the command line options for the `git checkout` command with this input:

snippet: sample_CheckoutInput

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

snippet: sample_CleanInput

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

snippet: sample_FileInput

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

snippet: sample_DictInput

In usage at the command line, the flag is used like this:

```
executable command --prop:color Red --prop:direction North
```

When this command line is parsed, the `PropFlag` property above will be a dictionary with the values
`color=Red` and `direction=North`.
