using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oakton.AspNetCore;

namespace MvcApp
{
    // SAMPLE: using-run-oakton-commands
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return CreateWebHostBuilder(args)
                
                // This extension method replaces the calls to
                // IWebHost.Build() and Start()
                .RunOaktonCommands(args);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        
    }
    // ENDSAMPLE
}
