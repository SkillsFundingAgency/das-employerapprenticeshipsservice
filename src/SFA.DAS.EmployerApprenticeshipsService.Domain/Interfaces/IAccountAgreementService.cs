using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAccountAgreementService
    {
        Task<int?> GetLatestSignedAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}