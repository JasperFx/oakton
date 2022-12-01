using System;
using System.IO;
using System.Linq;
using System.Net;
using JasperFx.StringExtensions;

namespace Oakton.Parsing
{
    public static class OptionReader
    {
        public static string Read(string file)
        {
            return File.ReadAllLines(file)
                .Select(x => x.Trim())
                .Where(x => x.IsNotEmpty())
                .Join(" ");
        }
    }
}