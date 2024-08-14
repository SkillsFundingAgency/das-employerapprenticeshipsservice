using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public interface IFinanceRepository
{
    Task<decimal> GetAccountBalance(string hashedAccountId);
}

public class FinanceRepository(IEmployerFinanceApiService financeApiService, ILogger<FinanceRepository> logger) : IFinanceRepository
{
    public async Task<decimal> GetAccountBalance(string hashedAccountId)
    {
        try
        {
            var response = await financeApiService.GetAccountBalances([hashedAccountId]);

            return response.First().Balance;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Account Balance with id {Id} not found", hashedAccountId);
            throw;
        }
    }
}