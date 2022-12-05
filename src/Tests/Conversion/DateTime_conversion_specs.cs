using System;
using Oakton.Internal.Conversion;
using Shouldly;
using Xunit;

namespace Tests.Conversion
{
    public class DateTime_conversion_specs
    {
        [Fact]
        public void get_date_time_for_day_and_time()
        {
            var date = DateTimeConverter.GetDateTime("Saturday 14:30");

            date.DayOfWeek.ShouldBe(DayOfWeek.Saturday);
            date.Date.AddHours(14).AddMinutes(30).ShouldBe(date);
            (date >= DateTime.Today).ShouldBe(true);
        }

        [Fact]
        public void get_date_time_for_day_and_time_2()
        {
            var date = DateTimeConverter.GetDateTime("Monday 14:30");

            date.DayOfWeek.ShouldBe(DayOfWeek.Monday);
            date.Date.AddHours(14).AddMinutes(30).ShouldBe(date);
            (date >= DateTime.Today).ShouldBe(true);
        }

        [Fact]
        public void get_date_time_for_day_and_time_3()
        {
            var date = DateTimeConverter.GetDateTime("Wednesday 14:30");

            date.DayOfWeek.ShouldBe(DayOfWeek.Wednesday);
            date.Date.AddHours(14).AddMinutes(30).ShouldBe(date);
            (date >= DateTime.Today).ShouldBe(true);
        }

        [Fact]
        public void get_date_time_from_full_iso_8601_should_be_a_utc_datetime()
        {
            var date = DateTimeConverter.GetDateTime("2012-06-01T14:52:35.0000000Z");

            date.ShouldBe(new DateTime(2012, 06, 01, 14, 52, 35, DateTimeKind.Utc));
        }

        [Fact]
        public void get_date_time_from_partial_iso_8601_uses_default_parser_and_is_local()
        {
            var date = DateTimeConverter.GetDateTime("2012-06-01T12:52:35Z");

            var gmtOffsetInHours = TimeZoneInfo.Local.GetUtcOffset(date).TotalHours;
            date.ShouldBe(new DateTime(2012, 06, 01, 12, 52, 35, DateTimeKind.Local).AddHours(gmtOffsetInHours));
        }

        [Fact]
        public void get_date_time_from_24_hour_time()
        {
            DateTimeConverter.GetDateTime("14:30").ShouldBe(DateTime.Today.AddHours(14).AddMinutes(30));
        }

        [Fact]
        public void parse_today()
        {
            DateTimeConverter.GetDateTime("TODAY").ShouldBe(DateTime.Today);
        }

        [Fact]
        public void parse_today_minus_date()
        {
            DateTimeConverter.GetDateTime("TODAY-3").ShouldBe(DateTime.Today.AddDays(-3));
        }

        [Fact]
        public void parse_today_plus_date()
        {
            DateTimeConverter.GetDateTime("TODAY+5").ShouldBe(DateTime.Today.AddDays(5));
        }
    }
}