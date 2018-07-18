using System;

namespace SFA.DAS.EAS.TestCommon.Extensions
{
    public static class DateTimeExtention
    {
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
        }

        public static string ToPeriodEndId(this DateTime date)
        {
            var yearAdjustment = 0;
            var monthAdjustment = -4;

            if (date.Month <= 4)
            {
                yearAdjustment = -1;
                monthAdjustment = 5;
            }

            return $"{date.AddYears(yearAdjustment):yy}{date.AddYears(yearAdjustment + 1):yy}-R{date.AddMonths(monthAdjustment):MM}";
        }
    }
}
