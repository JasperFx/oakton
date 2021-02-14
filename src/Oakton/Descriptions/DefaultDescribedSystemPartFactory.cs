using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.Descriptions
{
    public class DefaultDescribedSystemPartFactory : IDescribedSystemPartFactory
    {
        private readonly IServiceProvider _services;

        public DefaultDescribedSystemPartFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IDescribedSystemPart[] FindParts()
        {
            return _services.GetServices<IDescribedSystemPart>().ToArray();
        }
    }
}