using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Oakton.Resources;

#nullable disable annotations // FIXME

internal class ResourceSetupOptions
{
    public StartupAction Action { get; set; } = StartupAction.SetupOnly;
}

internal class ResourceSetupHostService : IHostedService
{
    private readonly ILogger<ResourceSetupHostService> _logger;
    private readonly ResourceSetupOptions _options;
    private readonly IStatefulResource[] _resources;
    private readonly IStatefulResourceSource[] _sources;

    public ResourceSetupHostService(ResourceSetupOptions options, IEnumerable<IStatefulResource> resources,
        IEnumerable<IStatefulResourceSource> sources, ILogger<ResourceSetupHostService> logger)
    {
        _resources = resources.ToArray();
        _sources = sources.ToArray();
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var list = new List<Exception>();
        var resources = new List<IStatefulResource>(_resources);

        foreach (var source in _sources)
        {
            try
            {
                resources.AddRange(source.FindResources());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to find resource sources from {Source}", source);
                list.Add(new ResourceSetupException(source, e));
            }
        }

        async ValueTask execute(IStatefulResource r, CancellationToken t)
        {
            try
            {
                await r.Setup(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Ran Setup() on resource {Name} of type {Type}", r.Name, r.Type);

                if (_options.Action == StartupAction.ResetState)
                {
                    await r.ClearState(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Ran ClearState() on resource {Name} of type {Type}", r.Name, r.Type);
                }
            }
            catch (Exception e)
            {
                var wrapped = new ResourceSetupException(r, e);
                _logger.LogError(e, "Failed to setup resource {Name} of type {Type}", r.Name, r.Type);

                list.Add(wrapped);
            }
        }

        foreach (var resource in resources) await execute(resource, cancellationToken).ConfigureAwait(false);

        if (list.Any())
        {
            throw new AggregateException(list);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}