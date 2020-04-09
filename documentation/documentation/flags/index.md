<!--title:Optional Flags-->

Flags are any public settable property or public field on the input class that are suffixed with 
*Flag*. Below is an example of several flags:

<[sample:CleanInput]>

## Setting Flag Values

Let's say you're trying to recreate the command line options for the `git checkout` command with this input:

<[sample:CheckoutInput]>

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
a property named `FirstNameFlag` would be parsed at the command line as either `-f` or `--first-name`.

You can of course override the flag alias in either long or short form by using the `[FlagAlias]` attribute as shown in the `CleanInput`
example above.

Lastly, if only the long form alias is desired, `[FlagAlias]` provides the constructor `FlagAliasAttribute(string longAlias, bool longAliasOnly)`.

Just like <[linkto:documentation/arguments]>, Flags can be any type that Oakton knows how to convert, with a few special types shown in the subsequent sections:


<[TableOfContents]>
