using System;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns DateTime for last day of month for given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonthDate(this DateTime date)
        {
            var lastDay = DateTime.DaysInMonth(date.Year, date.Month);

            return new DateTime(date.Year, date.Month, lastDay);
        }

        public static string ToFullDateEntryFormat(this DateTime date)
        {
            return date.ToString("dd MM yyyy");
        }

        /// <summary>
        /// GDS format: d MMM yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToGdsFormat(this DateTime date)
        {
            return date.ToString("d MMM yyyy");
        }

        /// <summary>
        /// GDS format: d MMMM yyyy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToGdsFormatFull(this DateTime date)
        {
            return date.ToString("d MMMM yyyy");
        }

        public static string ToGdsFormatLongMonthNameWithoutDay(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

        public static string ToGdsFormatOrdinalIndicator(this DateTime date)
        {
            var formattedDate = date.ToString("doo MMM yyyy");
            var day = date.Day;
            var remainder = day < 30 ? day % 20 : day % 30;
            var suffixes = new[] { "th", "st", "nd", "rd" };
            var suffix = remainder <= 3 ? suffixes[remainder] : suffixes[0];
            var result = formattedDate.Replace("oo", suffix);

            return result;
        }

        public static string ToGdsFormatWithoutDay(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

        public static string ToGdsFormatWithoutDayAbbrMonth(this DateTime date)
        {
            return date.ToString("MMM yyyy");
        }

        public static DateTime ToGMTStandardTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        }

        public static string ToRelativeFormat(this DateTime dateTime, DateTime relative)
        {
            var date = dateTime.Date;
            var today = relative.Date;
            var yesterday = today.AddDays(-1);

            if (date == today)
            {
                return "Today";
            }

            if (date == yesterday)
            {
                return "Yesterday";
            }

            var formattedDate = date.ToString("doo MMM yyyy");
            var day = date.Day;
            var remainder = day < 30 ? day % 20 : day % 30;
            var suffixes = new[] { "th", "st", "nd", "rd" };
            var suffix = remainder <= 3 ? suffixes[remainder] : suffixes[0];
            var result = formattedDate.Replace("oo", suffix);

            return result;
        }
    }
}