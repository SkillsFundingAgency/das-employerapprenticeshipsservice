using System;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IHmrcDateService
    {
        bool IsSubmissionDateInPayrollYear(string payrollYear, DateTime submissionDate);
        bool IsSubmissionEndOfYearAdjustment(string payrollYear, DateTime submissionDate);
    }
}