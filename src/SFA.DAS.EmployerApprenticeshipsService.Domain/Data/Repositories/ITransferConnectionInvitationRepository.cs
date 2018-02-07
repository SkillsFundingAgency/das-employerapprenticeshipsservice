using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferConnectionInvitationRepository
    {
        Task Add(TransferConnectionInvitation transferConnectionInvitation);
        Task<TransferConnectionInvitation> GetTransferConnectionInvitationToApproveOrReject(long transferConnectionInvitationId, long receiverAccountId);
    }
}