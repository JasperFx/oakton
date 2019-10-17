using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Baseline;
using Oakton.AspNetCore.Descriptions;
using Oakton.AspNetCore.Html;
using Shouldly;
using Xunit;

namespace Oakton.AspNetCore.Testing.Descriptions
{
    public class DescribeCommandTests
    {
        private readonly IDescribedSystemPart[] theParts = new[]
            {new DescribedPart(), new ConsoleWritingPart(), new HtmlTagWritingPart(),};


        [Fact]
        public async Task write_text()
        {
            var writer = new StringWriter();

            await DescribeCommand.WriteText(theParts, writer);
            
            writer.ToString().ShouldContain(theParts[0].As<DescribedPart>().Body);
            writer.ToString().ShouldContain(theParts[0].Title);
            
            writer.ToString().ShouldContain(theParts[1].As<DescribedPart>().Body);
            writer.ToString().ShouldContain(theParts[1].Title);
            
            writer.ToString().ShouldContain(theParts[1].As<DescribedPart>().Body);
            writer.ToString().ShouldContain(theParts[2].Title);
        }

        [Fact]
        public async Task write_to_console()
        {
            await DescribeCommand.WriteToConsole(theParts);
            
            theParts[1].As<ConsoleWritingPart>()
                .DidWriteToConsole.ShouldBeTrue();
        }

        [Fact]
        public void write_to_html()
        {
            var document = DescribeCommand.GenerateHtmlDocument(theParts);
            var html = document.ToString();
            
            html.ShouldContain(theParts[0].As<DescribedPart>().Body);
            html.ShouldContain(theParts[0].Title);
            
            html.ShouldContain(theParts[1].As<DescribedPart>().Body);
            html.ShouldContain(theParts[1].Title);
            
            html.ShouldContain(theParts[2].Key);
            html.ShouldContain($"id=\"{theParts[2].Key}\"");
        }
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

    public class ConsoleWritingPart : DescribedPart, IWriteToConsole
    {
        public void WriteToConsole()
        {
            DidWriteToConsole = true;
            
            Console.WriteLine(Body);
        }

        public bool DidWriteToConsole { get; set; }
    }

    public class HtmlTagWritingPart : DescribedPart, IDescribedByHtml
    {
        public HtmlTag Build()
        {
            return new HtmlTag("div").Id(Key);
        }
    }
}