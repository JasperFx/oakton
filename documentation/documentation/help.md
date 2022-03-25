<!--title:Help Text-->

Oakton comes with its own `[Description]` attribute that can be applied to fields or properties on
the input class to provide help information on usage like this:

snippet: sample_NameInput

or on the command class itself:

snippet: sample_NameCommand

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
