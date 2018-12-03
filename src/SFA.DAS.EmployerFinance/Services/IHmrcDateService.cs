using System;


namespace SFA.DAS.EmployerFinance.Services
{
    public interface IHmrcDateService
    {
        bool IsSubmissionDateInPayrollYear(string payrollYear, DateTime submissionDate);
        bool IsSubmissionEndOfYearAdjustment(string payrollYear, int payrollMonth, DateTime submissionDate);
        bool IsSubmissionForFuturePeriod(string payrollYear, int payrollMonth, DateTime dateProcessed);
        DateTime GetDateFromPayrollYearMonth(string payrollYear, int payrollMonth);
        bool DoesSubmissionPreDateLevy(string payrollYear);
        DateRange GetDateRangeForPayrollPeriod(string payrollYear, int payrollMonth);
        bool IsDateOntimeForPayrollPeriod(string payrollYear, int payrollMonth, DateTime dateTime);
        bool IsDateInPayrollPeriod(string payrollYear, int payrollMonth, DateTime dateTime);
    }
}