using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Baseline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Oakton.Resources;
using Shouldly;

namespace Tests.Resources
{
    public abstract class ResourceCommandContext
    {
        private IServiceCollection _services = new ServiceCollection();
        protected readonly List<IStatefulResource> AllResources = new List<IStatefulResource>();
        protected ResourceInput theInput = new ResourceInput();

        internal async Task theCommandExecutionShouldSucceed()
        {
            theInput.HostBuilder = Host.CreateDefaultBuilder().ConfigureServices(s => s.AddRange(_services));
            var returnCode = await new ResourceCommand().Execute(theInput);
            
            returnCode.ShouldBeTrue();
        }
        
        internal async Task theCommandExecutionShouldFail()
        {
            theInput.HostBuilder = Host.CreateDefaultBuilder().ConfigureServices(s => s.AddRange(_services));
            var returnCode = await new ResourceCommand().Execute(theInput);
            
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

            public Task<IReadOnlyList<IStatefulResource>> FindResources(CancellationToken token)
            {
                return Task.FromResult((IReadOnlyList<IStatefulResource>)_resources);
            }
        }
    }
}