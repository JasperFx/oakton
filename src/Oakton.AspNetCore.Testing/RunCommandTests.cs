using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace Oakton.AspNetCore.Testing
{
    public class RunCommandTests
    {
        [Fact]
        public async Task can_start_application()
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5111")
                .UseStartup<Startup>();
            
            var input = new AspNetCoreInput
            {
                WebHostBuilder = builder
            };
            
            var command = new RunCommand();

            var task = command.Execute(input);

            using (var client = new HttpClient())
            {
                var text = await client.GetStringAsync("http://localhost:5111");
                text.ShouldBe("Hello");
            }

            await command.Shutdown();

            await task;
        }
        
        
        public class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.Run(c => c.Response.WriteAsync("Hello"));
            }
        }
    }
    
    
}