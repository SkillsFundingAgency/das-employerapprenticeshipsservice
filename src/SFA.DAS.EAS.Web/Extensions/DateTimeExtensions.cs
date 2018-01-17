using System;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            var day = DateTime.DaysInMonth(date.Year, date.Month);

            return new DateTime(date.Year, date.Month, day);
        }

        public static string ToGdsFormat(this DateTime date)
        {
            return date.ToString("d MMM yyyy");
        }

        public static string ToGdsFormatFull(this DateTime date)
        {
            return date.ToString("d MMMM yyyy");
        }

        public static string ToGdsFormatWithoutDay(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

        public static string ToGdsFormatWithoutDayAbbrMonth(this DateTime date)
        {
            return date.ToString("MMM yyyy");
        }

        public static DateTime ToGmtStandardTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        }
    }
}