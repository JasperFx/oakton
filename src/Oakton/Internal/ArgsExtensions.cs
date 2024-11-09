using System.Linq;

namespace Oakton.Internal;

#nullable disable annotations // FIXME

public static class ArgsExtensions
{
    public static string[] FilterLauncherArgs(this string[] args)
    {
        if (args == null)
        {
            return new string[0];
        }

        while (args.Any() && args[0].StartsWith("%") && args[0].EndsWith("%"))
        {
            args = args.Skip(1).ToArray();
        }

        return args;
    }
}