using System;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class DateTimeExtensions
    {
        private const int financialYearStartDay = 20;
        private const int financialYearStartMonth = 4;

        public static string ToFinancialYearString(this DateTime dateTime)
        {
            var financialYearStartDate = new DateTime(DateTime.UtcNow.Year, financialYearStartMonth, financialYearStartDay);
            if(dateTime < financialYearStartDate)
            {
                return $"{dateTime.Year - 1}/{dateTime:yy}";
            }
            else
            {
                return $"{dateTime.Year}/{dateTime.AddYears(1):yy}";
            }
        }
    }
}
