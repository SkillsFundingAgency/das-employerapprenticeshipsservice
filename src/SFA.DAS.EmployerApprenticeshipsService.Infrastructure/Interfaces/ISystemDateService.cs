using System;

namespace SFA.DAS.EAS.Infrastructure.Interfaces
{
    /// <summary>
    ///     Service for obtaining what should be considered to be the current system date.
    /// </summary>
    /// <remarks>
    ///     This exists principally to allow <see cref="IFinancialYearDateCalculator"/> and <see cref="IDateService"/>  to be tested. 
    ///     It would be preferable to have the whole system use this service in preference to using DateTime.Now directly as it makes
    ///     date dependent testing easier. 
    /// </remarks>
    public interface ISystemDateService
    {
        DateTime Current { get; }
    }
}