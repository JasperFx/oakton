using System;
using System.IO;
using System.Threading.Tasks;
using Oakton;
using Oakton.Descriptions;
using Oakton.Html;

namespace MvcApp
{
    public class Describer1 : IDescribedSystemPart
    {
        public string Title { get; } = "The First Part";
        public string Key { get; } = "part1";
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

        public void WriteToConsole()
        {
            ConsoleWriter.Write(ConsoleColor.DarkBlue, "Second part writing in blue");
        }
    }

    public class Describer3 : IDescribedSystemPart, IDescribedByHtml
    {
        public string Title { get; } = "The Third Part";
        public string Key { get; } = "part3";
        public Task Write(TextWriter writer)
        {
            return writer.WriteLineAsync("Description of the third part");
        }

        public HtmlTag Build()
        {
            return new HtmlTag("h4").Style("color", "purple").Text("The third part Html");
        }
    }
}