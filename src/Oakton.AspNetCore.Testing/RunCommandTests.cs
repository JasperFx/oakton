using System.Net.Http;
using System.Threading.Tasks;
using Baseline.Dates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
#if NETCOREAPP2_2
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:5111")
                .UseStartup<Startup>();
#else
            var builder = new HostBuilder().ConfigureWebHostDefaults(x => 
            {
                x.UseKestrel()
                    .UseUrls("http://localhost:5111")
                    .UseStartup<Startup>();
            });
#endif
            
            var input = new RunInput
            {
                HostBuilder = builder
            };
            
            var command = new RunCommand();

            var task = Task.Factory.StartNew(() => command.Execute(input));

            command.Started.Wait(5.Seconds());
            
            using (var client = new HttpClient())
            {
                var text = await client.GetStringAsync("http://localhost:5111");
                text.ShouldBe("Hello");
            }

            command.Reset.Set();

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