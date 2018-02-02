using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public class DayMonthYear
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public static implicit operator DayMonthYear(DateTime dateTime)
        {
            return new DayMonthYear
            {
                Day = dateTime.Day,
                Month = dateTime.Month,
                Year = dateTime.Year
            };
        }

        public static implicit operator DateTime(DayMonthYear dayMonthYear)
        {
            return dayMonthYear.ToDate();
        }

        public bool IsValid()
        {
            return Day != null && Month != null && Year != null && DateTime.TryParse($"{Year}-{Month}-{Day}", out var _);
        }

        public DateTime ToDate()
        {
            return new DateTime(Year.Value, Month.Value, Day.Value, 0, 0, 0);
        }
    }
}