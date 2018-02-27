using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAccountAgreementService
    {
        Task<int?> GetLatestAgreementSignedByAccountAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}