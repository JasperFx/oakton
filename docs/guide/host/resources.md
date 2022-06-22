# Stateful Resources

:::tip
This feature was added in Oakton 4.5.0
:::

When you're working with the codebase of an application or service, you're also
likely to also be working with external infrastructure like databases or messaging
brokers. Taking the example of a database, at various times during development you
may want to:

* Set up the database schema from a brand new database installation
* Completely tear down the database schema to reclaim resources
* Clear all existing data out of the development database, but leave the schema in place
* Check that your code can indeed connect to the database
* Maybe interrogate the database for some kind of metrics that helps you test or troubleshoot your code

To that end, Oakton has the `IStatefulResource` model and the new `resources` command as
a way of interacting with these stateful resources like databases or messaging brokers
from the command line or even at application startup time.

## IStatefulResource Adapter

:::tip
Oakton assumes that there will be anywhere from 0 to many stateful resources in
your application.
:::

The first element is the `Oakton.Resources.IStatefulResource` interface shown below:

snippet: sample_IStatefulResource

You can create a new adapter for your infrastructure by implementing this interface and registering
a service in your .Net application's DI container. As an example, Jasper creates an `IStatefulResource`
adapter to [its Rabbit MQ integration](https://github.com/JasperFx/jasper/blob/36f86aa20634e8839d7d68838e2a9f5b2b023ef0/src/Jasper.RabbitMQ/Internal/RabbitMqTransport.Resource.cs) to allow Oakton to setup, teardown, purge, and check on 
the expected Rabbit MQ queues for an application.

The second abstraction is the smaller `Oakton.Resources.IStatefulResourceSource` that's just 
a helper to "find" other stateful resources. The [Weasel library](https://github.com/jasperfx/weasel) exposes the [DatabaseResources](https://github.com/JasperFx/weasel/blob/606099d2cbbb0505ea93b10af0118cfbeda20657/src/Weasel.CommandLine/DatabaseResources.cs)
adapter to "find" all the known Weasel managed databases to enable Oakton's stateful resource management.

snippet: sample_IStatefulResourceSource

To make the implementations easier, there is also an `Oakton.Resources.StatefulResourceBase` base
class you can use to make stateful resource adapters that only implement some of the possible
operations.

::: tip
Oakton automatically adds [environment checks](/guide/host/environment) for each stateful resource using its
`Check()` method
:::

## At Startup Time

Forget the command line for a second, if you have service registrations for `IStatefulResource`, you've got some
available tooling at runtime.

First, to just have your system automatically setup all resources on startup, use this option:

snippet: sample_using_AddResourceSetupOnStartup

The code above adds a custom `IHostedService` at the front of the line to call the `Setup()`
method on each registered `IStatefulResource` in your application.

The exact same functionality can be used with slightly different syntax:

snippet: sample_using_AddResourceSetupOnStartup2

Or, you can only have this applied when the system is running in "Development" mode:

snippet: sample_using_AddResourceSetupOnStartup3

## At Testing Time

There are some extension methods on `IHost` in the `Oakton.Resources` namespace
that you may find helpful at testing or development time:

snippet: sample_programmatically_control_resources


## "resources" Command

::: tip
The *list* option was added in Oakton 4.6.0.
:::

Because Oakton is primarily about command line usage, you can of course interact with 
your stateful resources through the command line using the `resources` command that's
automatically added with Oakton usage. If you'll type `dotnet run -- help resources` at the
command line of your application, you'll get this output:

```bash
resources - Check, setup, or teardown stateful resources of this system
├── Ensure all stateful resources are set up
│   └── dotnet run -- resources
│       ├── [-t, --timeout <timeout>]
│       ├── [-t, --type <type>]
│       ├── [-n, --name <name>]
│       ├── [-e, --environment <environment>]
│       ├── [-v, --verbose]
│       ├── [-l, --log-level <loglevel>]
│       └── [----config:<prop> <value>]
└── Execute an action against all resources
    └── dotnet run -- resources clear|teardown|setup|statistics|check|list
        ├── [-t, --timeout <timeout>]
        ├── [-t, --type <type>]
        ├── [-n, --name <name>]
        ├── [-e, --environment <environment>]
        ├── [-v, --verbose]
        ├── [-l, --log-level <loglevel>]
        └── [----config:<prop> <value>]


                              Usage   Description
──────────────────────────────────────────────────────────────────────────────────────────────────────────────
                             action   Resource action, default is setup
          [-t, --timeout <timeout>]   Timeout in seconds, default is 60
                [-t, --type <type>]   Optionally filter by resource type
                [-n, --name <name>]   Optionally filter by resource name
  [-e, --environment <environment>]   Use to override the ASP.Net Environment name
                    [-v, --verbose]   Write out much more information at startup and enables console logging
       [-l, --log-level <loglevel>]   Override the log level
        [----config:<prop> <value>]   Overwrite individual configuration items
```

You've got a couple of options. First, to just see what resources are registered
in your system, use:

```bash
dotnet run -- resources list
```

To simply check on the state of each of the resources, use:

```bash
dotnet run -- resources check
```

To set up all resources, use:

```bash
dotnet run -- resources setup
```

Likewise, to teardown all resources:

```bash
dotnet run -- resources teardown
```

Or clear any existing state:

```bash
dotnet run -- resources clear
```

Or finally just to see any statistics:

```bash
dotnet run -- resources statistics
```







