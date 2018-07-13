using System;

namespace SFA.DAS.EAS.Infrastructure.Interfaces
{
    /// <summary>
    ///     Service for performing date arithmetic regarding financial year start and end dates.
    /// </summary>
    public interface IFinancialYearDateCalculator
    {
        short GetEndFinancialYear(DateTime atPointInTime);
        DateTime GetYearStart(int endYear);
        DateTime GetYearEnd(int endYear);
    }
}