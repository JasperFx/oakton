using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

public class LambdaCheck : IEnvironmentCheck
{
    private readonly Func<IServiceProvider, CancellationToken, Task> _action;

    public LambdaCheck(string description, Func<IServiceProvider, CancellationToken, Task> action)
    {
        Description = description;
        _action = action;
    }

    public Task Assert(IServiceProvider services, CancellationToken cancellation)
    {
        return _action(services, cancellation);
    }

    public string Description { get; }

    public override string ToString()
    {
        return Description;
    }
}