using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Features
{
    public interface IAgreementService
    {
        Task<int?> GetAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}
