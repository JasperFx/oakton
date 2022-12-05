using System;
using Oakton.Internal.Conversion;
using Shouldly;
using Xunit;

namespace Tests.Conversion
{
    public class TimeSpanConverterTester
    {
        [Fact]
        public void happily_converts_timespans_in_4_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("1230").ShouldBe(new TimeSpan(12, 30, 0));
        }

        [Fact]
        public void happily_converts_timespans_in_5_digit_format()
        {
            TimeSpanConverter.GetTimeSpan("12:30").ShouldBe(new TimeSpan(12, 30, 0));
        }

        [Fact]
        public void converts_timespans_for_seconds()
        {
            TimeSpanConverter.GetTimeSpan("3.5s").ShouldBe(TimeSpan.FromSeconds(3.5));
            TimeSpanConverter.GetTimeSpan("5 s").ShouldBe(TimeSpan.FromSeconds(5));
            TimeSpanConverter.GetTimeSpan("1 second").ShouldBe(TimeSpan.FromSeconds(1));
            TimeSpanConverter.GetTimeSpan("12 seconds").ShouldBe(TimeSpan.FromSeconds(12));
        }

        [Fact]
        public void converts_timespans_for_minutes()
        {
            TimeSpanConverter.GetTimeSpan("10m").ShouldBe(TimeSpan.FromMinutes(10));
            TimeSpanConverter.GetTimeSpan("2.1 m").ShouldBe(TimeSpan.FromMinutes(2.1));
            TimeSpanConverter.GetTimeSpan("1 minute").ShouldBe(TimeSpan.FromMinutes(1));
            TimeSpanConverter.GetTimeSpan("5 minutes").ShouldBe(TimeSpan.FromMinutes(5));
        }

        [Fact]
        public void converts_timespans_for_hours()
        {
            TimeSpanConverter.GetTimeSpan("24h").ShouldBe(TimeSpan.FromHours(24));
            TimeSpanConverter.GetTimeSpan("4 h").ShouldBe(TimeSpan.FromHours(4));
            TimeSpanConverter.GetTimeSpan("1 hour").ShouldBe(TimeSpan.FromHours(1));
            TimeSpanConverter.GetTimeSpan("12.5 hours").ShouldBe(TimeSpan.FromHours(12.5));
        }

        [Fact]
        public void converts_timespans_for_days()
        {
            TimeSpanConverter.GetTimeSpan("3d").ShouldBe(TimeSpan.FromDays(3));
            TimeSpanConverter.GetTimeSpan("2 d").ShouldBe(TimeSpan.FromDays(2));
            TimeSpanConverter.GetTimeSpan("1 day").ShouldBe(TimeSpan.FromDays(1));
            TimeSpanConverter.GetTimeSpan("7 days").ShouldBe(TimeSpan.FromDays(7));
        }

        [Fact]
        public void can_convert_from_standard_format()
        {
            TimeSpanConverter.GetTimeSpan("00:00:01").ShouldBe(new TimeSpan(0, 0, 1));
            TimeSpanConverter.GetTimeSpan("00:10:00").ShouldBe(new TimeSpan(0, 10, 0));
            TimeSpanConverter.GetTimeSpan("01:30:00").ShouldBe(new TimeSpan(1, 30, 0));
            TimeSpanConverter.GetTimeSpan("1.01:30:00").ShouldBe(new TimeSpan(1, 1, 30, 0));
            TimeSpanConverter.GetTimeSpan("-00:10:00").ShouldBe(new TimeSpan(0, -10, 0));
            TimeSpanConverter.GetTimeSpan("12:34:56.789").ShouldBe(new TimeSpan(0, 12, 34, 56, 789));
        }
    }
}