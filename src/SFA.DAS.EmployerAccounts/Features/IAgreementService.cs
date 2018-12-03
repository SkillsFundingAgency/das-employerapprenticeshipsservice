using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Features
{
    public interface IAgreementService
    {
        Task<int?> GetAgreementVersionAsync(long accountId);
        Task RemoveFromCacheAsync(long accountId);
    }
}
