using System;
using System.Threading;
using System.Threading.Tasks;
using Oakton.Environment;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

internal class ResourceEnvironmentCheck : IEnvironmentCheck
{
    private readonly IStatefulResource _resource;

    public ResourceEnvironmentCheck(IStatefulResource resource)
    {
        _resource = resource;
    }

    public string Description => $"Resource {_resource.Name} ({_resource.Type})";

    public Task Assert(IServiceProvider services, CancellationToken cancellation)
    {
        return _resource.Check(cancellation);
    }
}