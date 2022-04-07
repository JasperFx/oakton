using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Oakton.Resources
{
    internal class ResourceSetupHostService : IHostedService
    {
        private readonly IStatefulResource[] _resources;
        private readonly IStatefulResourceSource[] _sources;
        private readonly ILogger<ResourceSetupHostService> _logger;

        public ResourceSetupHostService(IEnumerable<IStatefulResource> resources, IEnumerable<IStatefulResourceSource> sources, ILogger<ResourceSetupHostService> logger)
        {
            _resources = resources.ToArray();
            _sources = sources.ToArray();
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
                    resources.AddRange(await source.FindResources(cancellationToken));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to find resource sources from {Source}", source);
                    list.Add(new ResourceSetupException(source, e));
                }
            }

            Func<IStatefulResource, CancellationToken, ValueTask> execute = async (r, t) =>
            {
                try
                {
                    await r.Setup(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Ran setup on resource {Name} of type {Type}", r.Name, r.Type);
                }
                catch (Exception e)
                {
                    var wrapped = new ResourceSetupException(r, e);
                    _logger.LogError(e, "Failed to setup resource {Name} of type {Type}", r.Name, r.Type);

                    list.Add(wrapped);
                }
            };

#if NET6_0_OR_GREATER
            await Parallel.ForEachAsync(resources, cancellationToken, execute);
#else
            foreach (var resource in resources)
            {
                await execute(resource, cancellationToken).ConfigureAwait(false);
            }
#endif

            if (list.Any()) throw new AggregateException(list);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}