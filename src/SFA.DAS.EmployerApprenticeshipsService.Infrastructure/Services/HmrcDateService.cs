﻿using System;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcDateService : IHmrcDateService
    {
        public bool IsSubmissionDateInPayrollYear(string payrollYear, DateTime submissionDate)
        {
            DateTime endDate;
            var startDate = GetDateRange(payrollYear, out endDate);

            return submissionDate >= startDate && submissionDate <= endDate;
        }
        
        public bool IsSubmissionEndOfYearAdjustment(string payrollYear, int payrollMonth, DateTime submissionDate)
        {
            if (payrollMonth != 12)
            {
                return false;
            }

            DateTime endDate;
            GetDateRange(payrollYear, out endDate);
            endDate = endDate.AddDays(21);

            return submissionDate >= endDate;
        }

        public bool IsSubmissionForFuturePeriod(string payrollYear, int payrollMonth, DateTime dateProcessed)
        {
            var dateToCompare = GetDateFromPayrollYearMonth(payrollYear, payrollMonth);

            dateToCompare = dateToCompare.AddMonths(1);

            return dateToCompare > dateProcessed;
        }

        private DateTime GetDateRange(string payrollYear, out DateTime endDate)
        {
            var payrollSplit = payrollYear.Split('-');

            var startDate = new DateTime(Convert.ToInt32("20" + payrollSplit[0]), 4, 1);
            endDate = new DateTime(Convert.ToInt32("20" + payrollSplit[1]), 3, 31, 23, 59, 59);
            return startDate;
        }

        public DateTime GetDateFromPayrollYearMonth(string payrollYear, int payrollMonth)
        {
            var yearToUse = 2000;
            int monthToUse;

            var yearSplit = payrollYear.Split('-');

            if (payrollMonth >= 10)
            {
                yearToUse += Convert.ToInt32(yearSplit[1]);
                monthToUse = payrollMonth - 9;
            }
            else
            {
                yearToUse += Convert.ToInt32(yearSplit[0]);
                monthToUse = payrollMonth + 3;
            }
            
            return new DateTime(yearToUse,monthToUse,21);
        }

        public bool DoesSubmissionPreDateLevy(string payrollYear)
        {
            if (string.IsNullOrEmpty(payrollYear))
            {
                return false;
            }

            var yearSplit = payrollYear.Split('-');

            int result;
            if(int.TryParse(yearSplit[0], out result))
            {
                if (result <= 16)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
