using SFA.DAS.EmployerFinance.Models.TransferConnections;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface ITransferConnectionInvitationRepository
    {
        Task Add(TransferConnectionInvitation transferConnectionInvitation);
        Task<TransferConnectionInvitation> GetTransferConnectionInvitationById(int transferConnectionInvitationId);
    }
}