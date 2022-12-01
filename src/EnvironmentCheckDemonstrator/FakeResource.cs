using System;
using System.Threading;
using System.Threading.Tasks;
using Oakton.Resources;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace EnvironmentCheckDemonstrator
{
    public class FakeResource : IStatefulResource
    {
        private static Random _random = new Random();

        public FakeResource(string type, string name)
        {
            Type = type;
            Name = name;
        }
        
        public Exception Failure { get; set; }
        
        public int Delay { get; set; }

        public async Task Check(CancellationToken token)
        {
            await Task.Delay(_random.Next(100, 1000), token);
            await Task.Delay(Delay.Seconds(), token);
            if (Failure != null) throw Failure;
        }

        public async Task ClearState(CancellationToken token)
        {
            await Task.Delay(_random.Next(100, 1000), token);
            if (Failure != null) throw Failure;
        }

        public async Task Teardown(CancellationToken token)
        {
            await Task.Delay(_random.Next(100, 1000), token);
            if (Failure != null) throw Failure;
        }

        public async Task Setup(CancellationToken token)
        {
            await Task.Delay(_random.Next(100, 1000), token);
            if (Failure != null) throw Failure;
        }

        public async Task<IRenderable> DetermineStatus(CancellationToken token)
        {
            await Task.Delay(_random.Next(100, 1000), token);
            
            if (Failure != null) throw Failure;
            
            var table = new Table();
            table.AddColumns("Number", "Value");
            for (var i = 0; i < _random.Next(1, 10); i++)
            {
                table.AddRow(i.ToString(), Guid.NewGuid().ToString());
            }

            return table;
        }

        public string Type { get; }
        public string Name { get; }
    }
}