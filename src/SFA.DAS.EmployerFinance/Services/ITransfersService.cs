using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface ITransfersService
    {
        Task<GetCountsResponse> GetCounts(long accountId);
        Task<GetFinancialBreakdownResponse> GetFinancialBreakdown(long accountId);
    }
}