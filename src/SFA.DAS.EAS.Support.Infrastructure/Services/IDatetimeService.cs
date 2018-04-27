using System;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface IDatetimeService
    {
        int GetYear(DateTime endDate);
        DateTime GetBeginningFinancialYear(DateTime endDate);
    }
}