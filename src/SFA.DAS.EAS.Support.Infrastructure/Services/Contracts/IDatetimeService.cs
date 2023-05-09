namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface IDatetimeService
{
    int GetYear(DateTime endDate);
    DateTime GetBeginningFinancialYear(DateTime endDate);
}