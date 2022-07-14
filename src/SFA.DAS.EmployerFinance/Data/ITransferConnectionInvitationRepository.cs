using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransferConnectionInvitationRepository
    {
        Task Add(TransferConnectionInvitation transferConnectionInvitation);
        Task<TransferConnectionInvitation> GetTransferConnectionInvitationById(int transferConnectionInvitationId);
    }
}