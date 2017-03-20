using System;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class HmrcDateService
    {
        public bool IsSubmissionDateInPayrollYear(string payrollYear, DateTime submissionDate)
        {
            DateTime endDate;
            var startDate = GetDateRange(payrollYear, out endDate);

            return submissionDate >= startDate && submissionDate <= endDate;
        }
        
        public bool IsSubmissionEndOfYearAdjustment(string payrollYear, DateTime submissionDate)
        {
            DateTime endDate;
            GetDateRange(payrollYear, out endDate);

            return submissionDate >= endDate;
        }

        private static DateTime GetDateRange(string payrollYear, out DateTime endDate)
        {
            var payrollSplit = payrollYear.Split('-');

            var startDate = new DateTime(Convert.ToInt32("20" + payrollSplit[0]), 5, 1);
            endDate = new DateTime(Convert.ToInt32("20" + payrollSplit[1]), 4, 30, 23, 59, 59);
            return startDate;
        }
    }
}
