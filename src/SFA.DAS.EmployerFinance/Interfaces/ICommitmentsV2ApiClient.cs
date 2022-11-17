using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface ICommitmentsV2ApiClient
    {
        Task<GetApprenticeshipResponse> GetApprenticeship(long apprenticeshipId);
        Task<GetTransferRequestSummaryResponse> GetTransferRequests(long accountId);
    }
}
