<!--title:Enumerable Flags-->

You can use enumerable types like `string[]` or `IEnumerable<string>` for flag arguments to add multiple values. Flags are a little
more forgiving in this usage than arguments because Oakton can rely on the start of another flag to "know" when we've finished collecting
values for that array.

Let's say we have this input:

<[sample:FileInput]>

In usage, the flags could be used like:

```
executable command --files one.txt two.txt "c:\folder\file.txt" --directories c:\folder1 c:\folder2
```