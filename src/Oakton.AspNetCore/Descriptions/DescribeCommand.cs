using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Microsoft.Extensions.DependencyInjection;
using Oakton.AspNetCore.Html;

namespace Oakton.AspNetCore.Descriptions
{
    [Description("Writes out a description of your running application to either the console or a file")]
    public class DescribeCommand : OaktonAsyncCommand<DescribeInput>
    {
        public override async Task<bool> Execute(DescribeInput input)
        {
            using (var host = input.BuildHost())
            {
                var factory = host.Services.GetService<IDescribedSystemPartFactory>() ??
                              new DefaultDescribedSystemPartFactory(host.Services);

                var parts = factory.FindParts();

                if (input.ListFlag)
                {
                    Console.WriteLine("The registered system parts are");
                    foreach (var part in parts)
                    {
                        Console.WriteLine("* " + part.Key);
                    }

                    return true;
                }

                if (input.PartFlag.IsNotEmpty())
                {
                    parts = parts.Where(x => x.Key == input.PartFlag).ToArray();
                }

                if (!input.SilentFlag)
                {
                    await WriteToConsole(parts);
                }

                if (input.FileFlag.IsNotEmpty())
                {
                    if (input.HtmlFlag)
                    {
                        var document = GenerateHtmlDocument(parts);
                        var formatter = host.Services.GetService<IDescribedHtmlFormatter>();
                        formatter?.ApplyFormatting(document);
                        
                        File.WriteAllText(input.FileFlag, document.ToString());
                    }
                    else
                    {
                        using (var stream = new FileStream(input.FileFlag, FileMode.CreateNew, FileAccess.Write))
                        {
                            var writer = new StreamWriter(stream);
                            
                            await WriteText(parts, writer);
                            await writer.FlushAsync();
                        }
                        
                        
                    }
                    
                    Console.WriteLine("Wrote system description to file " + input.FileFlag);
                }

                return true;
            }
        }

        public static HtmlDocument GenerateHtmlDocument(IDescribedSystemPart[] parts)
        {
            var document = new HtmlDocument();
            document.Body.Add("a").Attr("name", "top");
            var tableOfContents = document.Body.Add("ol");
            document.Body.Add("hr");

            foreach (var part in parts)
            {
                tableOfContents.Add("li/a").Text(part.Title).Attr("href","#" + part.Key);

                document.Body.Add("a").Attr("name", part.Key);
                document.Body.Add("h3").Text(part.Title);

                if (part is IDescribedByHtml x)
                {
                    document.Body.Append(x.Build());
                }
                else
                {
                    var writer = new StringWriter();
                    part.Write(writer).GetAwaiter().GetResult();

                    document.Body.Add("pre").Text(writer.ToString());
                }

                document.Body.Add("a").Text("back to top...").Attr("href", "#top");

                document.Body.Add("hr");
            }





            return document;
        }

        public static async Task WriteText(IDescribedSystemPart[] parts, TextWriter writer)
        {
            foreach (var part in parts)
            {
                await writer.WriteLineAsync("##" + part.Title);
                await writer.WriteLineAsync();
                await part.Write(writer);
                await writer.WriteLineAsync();
                await writer.WriteLineAsync();
            }
        }

        public static async Task WriteToConsole(IDescribedSystemPart[] parts)
        {
            
            foreach (var part in parts)
            {
                ConsoleWriter.Write(ConsoleColor.Cyan, part.Title);
                Console.WriteLine();
                
                if (part is IWriteToConsole w)
                {
                    w.WriteToConsole();
                }
                else
                {
                    await part.Write(Console.Out);
                }
                
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}