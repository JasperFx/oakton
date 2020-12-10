using System.Net.Http;
using System.Threading.Tasks;
using Baseline.Dates;

using Microsoft.Extensions.Hosting;
using Shouldly;
using Xunit;

namespace Oakton.AspNetCore.Testing
{
    public class RunCommandTests
    {
        [Fact]
        public async Task can_start_application()
        {

            var builder = Host.CreateDefaultBuilder();

            
            var input = new RunInput
            {
                HostBuilder = builder
            };
            
            var command = new RunCommand();

            var task = Task.Factory.StartNew(() => command.Execute(input));

            command.Started.Wait(5.Seconds());

            command.Reset.Set();

            await task;
        }
        

    }
    
    
}