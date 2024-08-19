using Microsoft.Extensions.DependencyInjection;
using System;

namespace Oakton;

internal class DependencyInjectionCommandCreator : ICommandCreator
{
    private readonly IServiceProvider _serviceProvider;
    public DependencyInjectionCommandCreator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOaktonCommand CreateCommand(Type commandType)
    {
        var scope = _serviceProvider.CreateScope();
        return (IOaktonCommand)scope.ServiceProvider.GetRequiredService(commandType);
    }

    public object CreateModel(Type modelType)
    {
        return Activator.CreateInstance(modelType)!;
    }
}
