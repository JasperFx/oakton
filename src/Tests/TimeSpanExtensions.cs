using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Values are 0 to 2359
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static TimeSpan ToTime(this int minutes)
        {
            var text = minutes.ToString().PadLeft(4, '0');
            return text.ToTime();
        }

        public static TimeSpan ToTime(this string timeString)
        {
            
            return GetTimeSpan(timeString);
        }

        private const string TIMESPAN_PATTERN =
            @"
^(?<quantity>\d+    # quantity is expressed as some digits
(\.\d+)?)           # optionally followed by a decimal point or colon and more digits
\s*                 # optional whitespace
(?<units>[a-z]*)    # units is expressed as a word
$                   # match the entire string";


        public static TimeSpan GetTimeSpan(string timeString)
        {
            var match = Regex.Match(timeString.Trim(), TIMESPAN_PATTERN, RegexOptions.IgnorePatternWhitespace);
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
                int hours = int.Parse(timeString.Substring(0, 2));
                int minutes = int.Parse(timeString.Substring(2, 2));

                return new TimeSpan(hours, minutes, 0);
            }

            if (timeString.Length == 5 && timeString.Contains(":"))
            {
                var parts = timeString.Split(':');
                int hours = int.Parse(parts.ElementAt(0));
                int minutes = int.Parse(parts.ElementAt(1));

                return new TimeSpan(hours, minutes, 0);
            }

            throw new Exception("Time periods must be expressed in seconds, minutes, hours, or days.");
        }



        public static TimeSpan Minutes(this int number)
        {
            return new TimeSpan(0, 0, number, 0);
        }

        public static TimeSpan Hours(this int number)
        {
            return new TimeSpan(0, number, 0, 0);
        }

        public static TimeSpan Days(this int number)
        {
            return new TimeSpan(number, 0, 0, 0);
        }

        public static TimeSpan Seconds(this int number)
        {
            return new TimeSpan(0, 0, number);
        }


        public static TimeSpan Milliseconds(this int number)
        {
            return TimeSpan.FromMilliseconds(number);
        }
    }
}