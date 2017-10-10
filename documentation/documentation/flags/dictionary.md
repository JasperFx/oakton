<!--title:Key/Value Flags-->

<[info]>
Key/Value flags have to be of type `Dictionary<string, string>` or `IDictionary<string, string>`
<[/info]>

New to Oakton 1.3 is the ability to **finally** express key/value pairs as a special kind of flag. Let's say that we want
to capture extensible key/value pairs on our input class like this:

<[sample:DictInput]>

In usage at the command line, the flag is used like this:

```
executable command --prop:color Red --prop:direction North
```

When this command line is parsed, the `PropFlag` property above will be a dictionary with the values
`color=Red` and `direction=North`.