using System;

namespace SFA.DAS.EAS.Application.Messages
{
    public class Year : DayMonthYear
    {
        public Year()
        {
            Day = 1;
            Month = 1;
        }

        public static implicit operator Year(DateTime dateTime)
        {
            return new Year
            {
                Year = dateTime.Year
            };
        }

        public static implicit operator DateTime(Year year)
        {
            return year.ToDate();
        }
    }
}