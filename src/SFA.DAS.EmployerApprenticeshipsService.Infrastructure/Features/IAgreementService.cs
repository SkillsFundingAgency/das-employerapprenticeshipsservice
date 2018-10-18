using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public interface IAgreementService
    {
        Task<int?> GetAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}