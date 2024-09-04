using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Oakton.Help;

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
        //return new WrappedOaktonCommand(_serviceProvider, commandType);
    }

    public object CreateModel(Type modelType)
    {
        return Activator.CreateInstance(modelType)!;
    }
}

internal class WrappedOaktonCommand : IOaktonCommand
{
    private readonly IServiceScope _scope;
    private readonly IOaktonCommand _inner;

    public WrappedOaktonCommand(IServiceProvider provider, Type commandType)
    {
        _scope = provider.CreateScope();
        _inner = (IOaktonCommand)_scope.ServiceProvider.GetRequiredService(commandType);
    }

    public Type InputType => _inner.InputType;
    public UsageGraph Usages => _inner.Usages;
    public async Task<bool> Execute(object input)
    {
        try
        {
            return await _inner.Execute(input);
        }
        finally
        {
            _scope.SafeDispose();
        }
    }
}


