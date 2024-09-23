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
        return ActivatorUtilities.CreateInstance(_serviceProvider, commandType) as IOaktonCommand;
    }

    public object CreateModel(Type modelType)
    {
        return Activator.CreateInstance(modelType)!;
    }
}
