using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferRepository
    {
        Task<decimal> GetTransferAllowance(long accountId);
    }
}
