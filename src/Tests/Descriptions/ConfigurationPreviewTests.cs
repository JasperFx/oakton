using Baseline;
using Lamar.IoC.Instances;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.Descriptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Tests.Descriptions
{
    [Collection("SetConsoleOutput")]
    public class ConfigurationPreviewTests
    {
        [Fact]
        public async Task write_configuration_value_with_bracket_to_console()
        {
            // Arrange
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "NoBracketKey", "hello world" },
                { "BracketKey", "value with bracket [hello]" }
            });
            var config = configBuilder.Build();

            
            var original = Console.Out;
            var output = new StringWriter();
            Console.SetOut(output);

            try
            {
                // Act
                var preview = new ConfigurationPreview(config);
                await preview.WriteToConsole();


                // Assert
                var text = output.ToString().ReadLines();

                text.ShouldContain(x => x.Contains("hello world"));
                text.ShouldContain(x => x.Contains("value with bracket [hello]"));
            }
            finally
            {
                Console.SetOut(original);
            }
        }
    }
}
