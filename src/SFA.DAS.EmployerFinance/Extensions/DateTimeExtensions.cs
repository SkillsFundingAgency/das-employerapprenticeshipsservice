using System;

namespace SFA.DAS.EmployerFinance.Extensions
{
    public static class DateTimeExtensions
    {
        private const int FinancialYearStartDay = 20;
        private const int FinancialYearStartMonth = 4;

        public static string ToFinancialYearString(this DateTime dateTime, DateTime date)
        {
            DateTime FinancialYearStartDate = new DateTime(DateTime.UtcNow.Year, FinancialYearStartMonth, FinancialYearStartDay);
            if(dateTime < FinancialYearStartDate)
            {
                return $"{date.Year - 1}/{date.ToString("yy")}";
            }
            else
            {
                return $"{date.Year}/{date.AddYears(1).ToString("yy")}";
            }
        }
    }
}
