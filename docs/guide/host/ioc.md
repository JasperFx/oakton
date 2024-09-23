# Using IoC Services

Very frequently, folks have wanted to either use services from their IoC/DI container for their application, or to
have Oakton resolve the command objects from the application's DI container. New in Oakton 6.2 is that very ability. 

## Injecting Services into Commands

If you are using [Oakton's IHost integration](/oakton/guide/host/integration_with_i_host), you can write commands that
use IoC services by simply decorating a publicly settable property on your Oakton command classes with the
new `[InjectService]` attribute. 

First though, just to make sure you're clear about when and when this isn't applicable, this applies to Oakton used
from an `IHostBuilder` or `ApplicationBuilder` like so:

snippet: sample_using_ihost_activation

Then you can decorate your command types something like this:

snippet: sample_MyDbCommand

## Using IoC Command Creators


