using System;
using SFA.DAS.EAS.Infrastructure.Models;

namespace SFA.DAS.EAS.Infrastructure.Interfaces
{
    /// <summary>
    ///     Service for obtaining details of specific financial years.
    /// </summary>
    public interface IDateService
    {
        FinancialYearDetails PreviousFinancialYear { get; }
        FinancialYearDetails CurrentFinancialYear { get; }
        FinancialYearDetails NextFinancialYear { get; }
        FinancialYearDetails GetFinancialYear(DateTime atPointInTime);
        FinancialYearDetails GetFinancialYearEnding(int endingInYear);
        FinancialYearDetails GetFinancialYearStarting(int startingInYear);
    }
}
