<!--title:Boolean Flags-->

Boolean flags are just a little bit different because there's no value necessary. If the flag appears in the command line arguments,
the field or property is set to `true`.

Consider our recreation of the `git clean` command:

<[sample:CleanInput]>

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