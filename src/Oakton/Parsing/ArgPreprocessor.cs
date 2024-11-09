using System.Collections.Generic;

namespace Oakton.Parsing;

#nullable disable annotations // FIXME

public class ArgPreprocessor
{
    public static IEnumerable<string> Process(IEnumerable<string> incomingArgs)
    {
        var newArgs = new List<string>();

        foreach (var arg in incomingArgs)
        {
            if (isMultiArg(arg))
            {
                foreach (var c in arg.TrimStart('-')) newArgs.Add("-" + c);
            }
            else
            {
                newArgs.Add(arg);
            }
        }

        return newArgs;
    }

    private static bool isMultiArg(string arg)
    {
        // Getting around GH-24
        if (decimal.TryParse(arg, out var number))
        {
            return false;
        }

        // regular short args look like '-a', multi-args are '-abc' which is really '-a -b -c'
        return InputParser.IsShortFlag(arg) && arg.Length > 2;
    }
}