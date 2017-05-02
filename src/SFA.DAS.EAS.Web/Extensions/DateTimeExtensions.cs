using System;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DateTimeExtensions
    {
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

        public static string ToFullDateEntryFormat(this DateTime date)
        {
            return date.ToString("dd MM yyyy");
        }

        public static DateTime ToGMTStandardTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        }
        public static string ToGdsFormatWithoutDayAbbrMonth(this DateTime date)
        {
            return date.ToString("MMM yyyy");
        }
        public static string ToGdsFormatWithoutDay(this DateTime date)
        {
            return date.ToString("MMMM yyyy");
        }

    }
}