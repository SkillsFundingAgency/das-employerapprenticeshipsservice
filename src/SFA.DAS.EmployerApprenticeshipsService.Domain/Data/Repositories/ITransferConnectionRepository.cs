using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.TransferConnection;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferConnectionRepository
    {
        Task<long> Create(TransferConnection transferConnection);
        Task<TransferConnection> GetCreatedTransferConnection(long id);
        Task<TransferConnection> GetSentTransferConnection(long id);
        Task Send(TransferConnection transferConnection);
    }
}