using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.TransferConnection;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferConnectionInvitationRepository
    {
        Task<long> Create(TransferConnectionInvitation transferConnectionInvitation);
        Task<TransferConnectionInvitation> GetCreatedTransferConnectionInvitation(long id);
        Task<TransferConnectionInvitation> GetSentTransferConnectionInvitation(long id);
        Task Send(TransferConnectionInvitation transferConnectionInvitation);
    }
}