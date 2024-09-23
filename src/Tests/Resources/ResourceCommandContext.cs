using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Oakton.Resources;
using Shouldly;
using Spectre.Console.Rendering;

namespace Tests.Resources
{
    public abstract class ResourceCommandContext
    {
        private IServiceCollection _services = new ServiceCollection();
        protected readonly List<IStatefulResource> AllResources = new List<IStatefulResource>();
        protected ResourceInput theInput = new ResourceInput();

        internal void CopyResources(IServiceCollection services)
        {
            services.AddRange(_services);
        }

        internal Task<IHost> buildHost()
        {
            return Host.CreateDefaultBuilder().ConfigureServices(CopyResources).StartAsync();
        }
        
        internal IList<IStatefulResource> applyTheResourceFiltering()
        {
            theInput.HostBuilder = Host.CreateDefaultBuilder().ConfigureServices(CopyResources);
            var command = new ResourcesCommand();
            using var host = theInput.BuildHost();

            return command.FindResources(theInput, host);
        }
        
        internal async Task theCommandExecutionShouldSucceed()
        {
            theInput.HostBuilder = Host.CreateDefaultBuilder().ConfigureServices(CopyResources);
            var returnCode = await new ResourcesCommand().Execute(theInput);
            
            returnCode.ShouldBeTrue();
        }
        
        internal async Task theCommandExecutionShouldFail()
        {
            theInput.HostBuilder = Host.CreateDefaultBuilder().ConfigureServices(s => s.AddRange(_services));
            var returnCode = await new ResourcesCommand().Execute(theInput);
            
            returnCode.ShouldBeFalse();
        }
        
        
        internal IStatefulResource CreateResource(string name, string type = "Resource")
        {
            var resource = Substitute.For<IStatefulResource>();
            resource.Name.Returns(name);
            resource.Type.Returns(type);
            
            AllResources.Add(resource);

            return resource;
        }

        internal void AddSource(Action<ResourceCollection> configure)
        {
            var collection = new ResourceCollection(this);
            configure(collection);

            _services.AddSingleton<IStatefulResourceSource>(collection);
        }

        internal IStatefulResource AddResource(string name, string type = "Resource")
        {
            var resource = CreateResource(name, type);
            _services.AddSingleton<IStatefulResource>(resource);
            return resource;
        }
        
        internal IStatefulResource AddResourceWithDependencies(string name, string type, params string[] dependencyNames)
        {
            var resource = new ResourceWithDependencies
            {
                Name = name,
                Type = type,
                DependencyNames = dependencyNames
            };
            _services.AddSingleton<IStatefulResource>(resource);
            return resource;
        }

        public class ResourceCollection : IStatefulResourceSource
        {
            private readonly ResourceCommandContext _parent;
            private readonly List<IStatefulResource> _resources = new List<IStatefulResource>();

            public ResourceCollection(ResourceCommandContext parent)
            {
                _parent = parent;
            }

            public IStatefulResource Add(string name, string type = "Resource")
            {
                var resource = _parent.CreateResource(name, type);
                _resources.Add(resource);

                return resource;
            }

            public IReadOnlyList<IStatefulResource> FindResources()
            {
                return _resources;
            }
        }
    }
}

public class ResourceWithDependencies : IStatefulResourceWithDependencies
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string[] DependencyNames { get; set; }

    public Task Check(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task ClearState(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task Teardown(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task Setup(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    public Task<IRenderable> DetermineStatus(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IStatefulResource> FindDependencies(IReadOnlyList<IStatefulResource> others)
    {
        return others.Where(x => DependencyNames.Contains(x.Name));
    }
}