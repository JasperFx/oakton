using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Oakton.Descriptions
{
    [Description("Writes out a description of your running application to either the console or a file")]
    public class DescribeCommand : OaktonAsyncCommand<DescribeInput>
    {
        public override async Task<bool> Execute(DescribeInput input)
        {
            using (var host = input.BuildHost())
            {
                var factories = host.Services.GetServices<IDescribedSystemPartFactory>();
                var parts = host.Services.GetServices<IDescribedSystemPart>()
                    .Concat(factories.SelectMany(x => x.Parts())).ToArray();

                foreach (var partWithServices in parts.OfType<IRequiresServices>())
                {
                    partWithServices.Resolve(host.Services);
                }

                if (input.ListFlag)
                {
                    Console.WriteLine("The registered system parts are");
                    foreach (var part in parts)
                    {
                        Console.WriteLine("* " + part.Title);
                    }

                    return true;
                }

                if (input.TitleFlag.IsNotEmpty())
                {
                    parts = parts.Where(x => x.Title == input.TitleFlag).ToArray();
                }

                if (!input.SilentFlag)
                {
                    await WriteToConsole(parts);
                }

                if (input.FileFlag.IsNotEmpty())
                {
                    using (var stream = new FileStream(input.FileFlag, FileMode.CreateNew, FileAccess.Write))
                    {
                        var writer = new StreamWriter(stream);
                            
                        await WriteText(parts, writer);
                        await writer.FlushAsync();
                    }

                    Console.WriteLine("Wrote system description to file " + input.FileFlag);
                }

                return true;
            }
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
                var rule = new Rule($"[blue]{part.Title}[/]")
                {
                    Alignment = Justify.Left,
                };
                
                AnsiConsole.Render(rule);

                if (part is IWriteToConsole o)
                {
                    await o.WriteToConsole();
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