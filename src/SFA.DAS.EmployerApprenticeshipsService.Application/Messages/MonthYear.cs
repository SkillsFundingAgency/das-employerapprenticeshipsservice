using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public class MonthYear : DayMonthYear
    {
        public MonthYear()
        {
            Day = 1;
        }

        public static implicit operator MonthYear(DateTime dateTime)
        {
            return new MonthYear
            {
                Month = dateTime.Month,
                Year = dateTime.Year
            };
        }

        public static implicit operator DateTime(MonthYear monthYear)
        {
            return monthYear.ToDate();
        }
    }
}