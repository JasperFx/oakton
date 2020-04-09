<!--title:Arguments-->

An argument is any public field or settable property on the input class that is not suffixed with "Flag" or marked
with the `[IgnoreOnCommandLine]`. Below is a sample:

<[sample:sample-arguments]>

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

<[sample:EnumerableArguments]>

There's one huge caveat though, **an enumerable or array variable will need to be the very last argument** in your command line usage.

The usage would be:

```
executable command value1 value2 value3 --flag --flag2
```

## Valid Argument Patterns

Look at the input class from the <[linkto:documentation/getting_started]> tutorial:

<[sample:NameInput]>

In this particular case, I want users to be able to enter either the `Name` argument or both the `Name` and
`Color` arguments. In the matching command class for this input, I specify two `Usages` patterns for this command:

<[sample:NameCommand]>

The call to `Usage` in the constructor function specifies the valid argument patterns for the command. If that explicit
`Usage` configuration was omitted, Oakton assumes that every argument is always mandatory and appears in the order of the
properties or fields in the input class.


