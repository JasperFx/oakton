using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Oakton.Descriptions
{
    internal class LambdaDescribedSystemPart<T> : IDescribedSystemPart, IRequiresServices
    {
        public string Title { get; }
        private readonly Func<T, TextWriter, Task> _write;
        private T _service;

        public LambdaDescribedSystemPart(string title, Func<T, TextWriter, Task> write)
        {
            Title = title;
            _write = write;
        }

        public void Resolve(IServiceProvider services)
        {
            _service = services.GetService<T>();
        }

        public Task Write(TextWriter writer)
        {
            return _service == null ? default : _write(_service, writer);
        }

        
    }
}