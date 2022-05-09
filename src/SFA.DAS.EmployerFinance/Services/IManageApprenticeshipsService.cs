using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IManageApprenticeshipsService
    {
        Task<GetIndexResponse> GetIndex(long accountId);
        Task<GetFinancialBreakdownResponse> GetFinancialBreakdown(long accountId);
    }
}