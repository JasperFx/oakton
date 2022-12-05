using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JasperFx.Core;
using Microsoft.Extensions.Configuration;
using Oakton.Descriptions;
using Xunit;

namespace Tests.Descriptions;

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
        
        }
        finally
        {
            Console.SetOut(original);
        }
    }
}