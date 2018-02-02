using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferRepository
    {
        Task<double> GetTransferBalance(string hashedAccountId);
    }
}
