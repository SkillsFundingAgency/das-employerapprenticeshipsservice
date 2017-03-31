using System;
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

            return submissionDate >= endDate;
        }

        public bool IsSubmissionForFuturePeriod(string payroll, int payrollMonth, DateTime submissionDate)
        {
            throw new NotImplementedException();
        }

        private static DateTime GetDateRange(string payrollYear, out DateTime endDate)
        {
            var payrollSplit = payrollYear.Split('-');

            var startDate = new DateTime(Convert.ToInt32("20" + payrollSplit[0]), 4, 1);
            endDate = new DateTime(Convert.ToInt32("20" + payrollSplit[1]), 3, 31, 23, 59, 59);
            return startDate;
        }

        private static DateTime GetDateFromPayrollYearMonth(string payroll, int payrollMonth)
        {
            
            return new DateTime();
        }
    }
}
