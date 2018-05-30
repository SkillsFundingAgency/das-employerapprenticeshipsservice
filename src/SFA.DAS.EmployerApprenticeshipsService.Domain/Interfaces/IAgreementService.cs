using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAgreementService
    {
        Task<int?> GetAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}