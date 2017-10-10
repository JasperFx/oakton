<!--title:"Opts" Files-->

<[info]>
This feature is all new in Oakton and was inspired by Javascript tools like Mocha that use "opts" files to make
their tools much easier to use at the command line
<[/info]>

As a contrived example (that probably violates all kind of security best practices), let's say that your console application exposes several commands, but all of the commands may need
an optional user name and password flag. You might start with a base class for your command inputs like this:

<[sample:SecuredInput]>

To make the tool easier to use, we can take advantage of the "opts" file option by making Oakton look for the presence of an optional text file in the same directory as the command execution with a certain name to pick up default command usages.

We can set that up by declaring the name of the opts file like this:

<[sample:configuring-opts-file]>

Now, we could have add a file named `mytool.opts` to the directory where we run the command with this sample content:

```
-u MyUserName
-p ~~HeMan2345
```

Now, when we run the command line `mytool somecommand` and that opts file is found in the current directory, Oakton appends the data of each line in that file so that the executed command is really `mytool somecommand -u MyUserName -p ~~HeMan2345`. 

A couple other things to note:

* The presence of the named opts file is not mandatory
* You can express arguments (maybe not super useful) or more likely any number of flag values
* The opts file can be one or more lines if that aids readability


