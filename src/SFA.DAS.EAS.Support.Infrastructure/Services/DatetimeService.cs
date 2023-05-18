using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public class DatetimeService : IDatetimeService
{
    private const int FirstFiscalYearMonth = 4;

    public int GetYear(DateTime endDate)
    {
        return endDate.Month >= FirstFiscalYearMonth ? endDate.Year : endDate.Year - 1;
    }

    public DateTime GetBeginningFinancialYear(DateTime endDate)
    {
        return new DateTime(GetYear(endDate), FirstFiscalYearMonth, 1);
    }
}