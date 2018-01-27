using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferConnectionInvitationRepository
    {
        Task<long> Add(TransferConnectionInvitation transferConnectionInvitation);
        Task<TransferConnectionInvitation> GetSentTransferConnectionInvitation(long id);
        Task<IEnumerable<TransferConnectionInvitation>> GetTransferConnectionInvitations(long senderAccountId, long receiverAccountId);
    }
}