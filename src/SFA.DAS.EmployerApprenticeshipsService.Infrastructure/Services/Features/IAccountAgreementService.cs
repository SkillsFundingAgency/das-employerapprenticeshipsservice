using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public interface IAccountAgreementService
    {
        Task<decimal?> GetLatestAgreementSignedByAccountAsync(long accountId);
    }
}