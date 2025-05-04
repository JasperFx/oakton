using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Oakton.Internal.Conversion;

#nullable disable annotations // FIXME

public class TimeSpanConverter
{
    private const string TimespanPattern =
        @"
^(?<quantity>\d+    # quantity is expressed as some digits
(\.\d+)?)           # optionally followed by a decimal point or colon and more digits
\s*                 # optional whitespace
(?<units>[a-z]*)    # units is expressed as a word
$                   # match the entire string";


    public static TimeSpan GetTimeSpan(string timeString)
    {
        var match = Regex.Match(timeString.Trim(), TimespanPattern, RegexOptions.IgnorePatternWhitespace);
        if (!match.Success)
        {
            return TimeSpan.Parse(timeString);
        }


        var number = double.Parse(match.Groups["quantity"].Value);
        var units = match.Groups["units"].Value.ToLower();
        switch (units)
        {
            case "s":
            case "second":
            case "seconds":
                return TimeSpan.FromSeconds(number);

            case "m":
            case "minute":
            case "minutes":
                return TimeSpan.FromMinutes(number);

            case "h":
            case "hour":
            case "hours":
                return TimeSpan.FromHours(number);

            case "d":
            case "day":
            case "days":
                return TimeSpan.FromDays(number);
        }

        if (timeString.Length == 4 && !timeString.Contains(":"))
        {
            var hours = int.Parse(timeString.Substring(0, 2));
            var minutes = int.Parse(timeString.Substring(2, 2));

            return new TimeSpan(hours, minutes, 0);
        }

        if (timeString.Length == 5 && timeString.Contains(":"))
        {
            var parts = timeString.Split(':');
            var hours = int.Parse(parts.ElementAt(0));
            var minutes = int.Parse(parts.ElementAt(1));

            return new TimeSpan(hours, minutes, 0);
        }

        throw new Exception("Time periods must be expressed in seconds, minutes, hours, or days.");
    }
}