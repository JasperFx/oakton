# Using IoC Services

Very frequently, folks have wanted to either use services from their IoC/DI container for their application, or to
have Oakton resolve the command objects from the application's DI container. New in Oakton 6.2 is that very ability. 

## Injecting Services into Commands

If you are using [Oakton's IHost integration](/guide/host/integration_with_i_host), you can write commands that
use IoC services by simply decorating a publicly settable property on your Oakton command classes with the
new `[InjectService]` attribute. 

First though, just to make sure you're clear about when and when this isn't applicable, this applies to Oakton used
from an `IHostBuilder` or `ApplicationBuilder` like so:

snippet: sample_using_ihost_activation

Then you can decorate your command types something like this:

snippet: sample_MyDbCommand

Just know that when you do this and execute a command that has decorated properties for services, Oakton is:

1. Building your system's `IHost`
2. Creating a new `IServiceScope` from your application's DI container, or in other words, a scoped container
3. Building your command object and setting all the dependencies on your command object by resolving each dependency from the scoped container created in the previous step
4. Executing the command as normal
5. Disposing the scoped container and the `IHost`, effectively in a `try/finally` so that Oakton is always cleaning up after the application


## Using IoC Command Creators

If you would like to just always have Oakton try to use dependency injection in the constructor of the command classes,
you also have this option. First, register Oakton through the application's DI container and run the Oakton commands through
the `IHost.RunHostedOaktonCommands()` extension method as shown below:




