using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ITransferAllowanceSnapshotRepository
    {
        Task UpsertAsync(long accountId, int year, decimal amount);
    }
}