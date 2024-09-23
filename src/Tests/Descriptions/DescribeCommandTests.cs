using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.Descriptions;
using Shouldly;
using Spectre.Console;
using Xunit;

namespace Tests.Descriptions
{
    public class DescribeCommandTests
    {
        private readonly IDescribedSystemPart[] theParts = new[]
            {new DescribedPart(), new ConsoleWritingPart(),};

        

        [Fact]
        public async Task write_text()
        {
            var writer = new StringWriter();

            await DescribeCommand.WriteText(theParts, writer);
            
            writer.ToString().ShouldContain(theParts[0].As<DescribedPart>().Body);
            writer.ToString().ShouldContain(theParts[0].Title);
            
            writer.ToString().ShouldContain(theParts[1].As<DescribedPart>().Body);
            writer.ToString().ShouldContain("## " + theParts[1].Title);
        }

        [Fact]
        public async Task write_to_console()
        {
            await DescribeCommand.WriteToConsole(theParts);
            
            theParts[1].As<ConsoleWritingPart>()
                .DidWriteToConsole.ShouldBeTrue();
        }

        [Fact]
        public async Task use_lambda_description()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(new FakeService {Name = "Hugo"});
                    services.Describe<FakeService>("Fake Service #1", (s, w) => w.WriteLine("Fake Service Name: " + s.Name));
                    services.Describe<FakeService>("Fake Service #2", (s, w) => w.WriteLineAsync("Fake Service Name Async: " + s.Name));
                });

            var input = new DescribeInput
            {
                HostBuilder = builder
            };

            var original = Console.Out;
            var output = new StringWriter();
            Console.SetOut(output);

            try
            {
                await new DescribeCommand().Execute(input);

                var text = output.ToString().ReadLines();
                
                //text.ShouldContain("Fake Service #1");
                text.ShouldContain("Fake Service Name: Hugo");
                
                //text.ShouldContain("Fake Service #2");
                text.ShouldContain("Fake Service Name Async: Hugo");
            }
            finally
            {
                Console.SetOut(original);
            }

        }

        [Fact]
        public async Task using_custom_part_factory()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(new FakeService {Name = "Hugo"});
                    services.AddDescriptionFactory<CustomDiscovery>();
                });

            var input = new DescribeInput
            {
                HostBuilder = builder
            };

            var original = Console.Out;
            var output = new StringWriter();
            Console.SetOut(output);

            try
            {
                await new DescribeCommand().Execute(input);

                var text = output.ToString().ReadLines();

            }
            finally
            {
                Console.SetOut(original);
            }
        }
    }

    public class CustomDiscovery : IDescribedSystemPartFactory
    {
        public IDescribedSystemPart[] Parts()
        {
            return new IDescribedSystemPart[] {new Describer1(), new Describer2(), new Describer3()};
        }
    }
    
    public class Describer1 : IDescribedSystemPart
    {
        public string Title { get; } = "The First Part";

        public Task Write(TextWriter writer)
        {
            return writer.WriteLineAsync("Description of the first part");
        }
    }

    public class Describer2 : IDescribedSystemPart, IWriteToConsole
    {
        public string Title { get; } = "The Second Part";
        public string Key { get; } = "part2";
        public Task Write(TextWriter writer)
        {
            return writer.WriteLineAsync("Description of the second part");
        }

        public Task WriteToConsole()
        {
            AnsiConsole.MarkupLine("[darkblue]Second part writing in blue[/]");
            return Task.CompletedTask;
        }
    }

    public class Describer3 : IDescribedSystemPart
    {
        public string Title { get; } = "The Third Part";
        public string Key { get; } = "part3";
        public Task Write(TextWriter writer)
        {
            return writer.WriteLineAsync("Description of the third part");
        }

    }

    public class FakeService
    {
        public string Name { get; set; }
    }

    public class DescribedPart : IDescribedSystemPart
    {
        public string Title { get; set; } = Guid.NewGuid().ToString();
        public string Key { get; set; } = Guid.NewGuid().ToString();
        public string Body { get; set; } = Guid.NewGuid().ToString();
        public Task Write(TextWriter writer)
        {
            return writer.WriteLineAsync(Body);
        }
    }

    public class DescribedAndTablePart : DescribedPart, IWriteToConsole, IDescribesProperties
    {
        public bool DidWriteToConsole { get; set; }
        
        public Task WriteToConsole()
        {
            DidWriteToConsole = true;

            var table = DescribeProperties().BuildTableForProperties();
            AnsiConsole.Write(table);
            
            return Task.CompletedTask;
        }

        public IDictionary<string, object> DescribeProperties()
        {
            return new Dictionary<string, object>
            {
                {"foo", "bar"},
                {"number", 5},
            };
        }
    }

    public class ConsoleWritingPart : DescribedPart, IWriteToConsole
    {
        public Task WriteToConsole()
        {
            DidWriteToConsole = true;
            
            Console.WriteLine(Body);

            return Task.CompletedTask;
        }

        public bool DidWriteToConsole { get; set; }
    }

}