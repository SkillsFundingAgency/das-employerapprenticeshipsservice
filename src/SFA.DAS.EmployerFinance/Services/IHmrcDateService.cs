using System;


namespace SFA.DAS.EmployerFinance.Services
{
    public interface IHmrcDateService
    {
        bool IsSubmissionDateInPayrollYear(string payrollYear, DateTime submissionDate);
        bool IsSubmissionEndOfYearAdjustment(string payrollYear, int payrollMonth, DateTime submissionDate);
        bool IsSubmissionForFuturePeriod(string payrollYear, int payrollMonth, DateTime dateProcessed);

        /// <summary>
        ///     Returns the payroll date for specified payroll month and period. So for example,
        ///     if the year if "18-19" and Period is 1 (period 1 being April) 20th April 2018
        ///     will be returned. 
        /// </summary>
        DateTime GetDateFromPayrollYearMonth(string payrollYear, int payrollMonth);
        bool DoesSubmissionPreDateLevy(string payrollYear);

        /// <summary>
        ///     Returns an object that contains the payroll date for a period (<see cref="GetDateFromPayrollYearMonth"/>)
        ///     and the last instant that a submission may be made before it is considered late.
        /// </summary>
        DateRange GetDateRangeForPayrollPeriod(string payrollYear, int payrollMonth);
        bool IsDateInPayrollPeriod(string payrollYear, int payrollMonth, DateTime dateTime);
    }
}