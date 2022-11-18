using System;
using System.Linq;

namespace Oakton.Parsing
{
    public static class OptionReader
    {
        public static string Read(string file)
        {
            return new FileSystem()
                .ReadStringFromFile(file)
                .ReadLines()
                .Select(x => x.Trim())
                .Where(x => x.IsNotEmpty())
                .Join(" ");

        }
    }
}