using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcApp.Models;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public HomeController(IHostingEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var writer = new StringWriter();
            writer.WriteLine("Hey, it's me!");
            writer.WriteLine($"Environment: {_environment.EnvironmentName}");
            
            writer.WriteLine($"Color: {_configuration["color"]}");
            writer.WriteLine($"Number: {_configuration["number"]}");
            
            return Content(writer.ToString(), "text/plain");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
