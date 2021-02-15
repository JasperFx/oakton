using System;
using System.IO;
using System.Threading.Tasks;
using Oakton;
using Oakton.Descriptions;

namespace EnvironmentCheckDemonstrator
{
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
            ConsoleWriter.Write(ConsoleColor.DarkBlue, "Second part writing in blue");
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
}