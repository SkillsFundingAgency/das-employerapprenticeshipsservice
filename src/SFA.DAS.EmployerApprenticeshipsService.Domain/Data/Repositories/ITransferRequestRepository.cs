using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.TransferRequests;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferRequestRepository
    {
        Task Add(TransferRequest transferRequest);
        Task<TransferRequest> GetTransferRequestByCommitmentId(long commitmentId);
    }
}