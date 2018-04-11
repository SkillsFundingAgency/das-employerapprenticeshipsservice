using System.Globalization;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class AccountAgreementService : IAccountAgreementService
    {
        private const int StoredValueThatMeansNull = -1;

        private readonly IDistributedCache _cache;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public AccountAgreementService(IDistributedCache cache, IEmployerAgreementRepository employerAgreementRepository)
        {
            _cache = cache;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<int?> GetLatestAgreementSignedByAccountAsync(long accountId)
        {
            var latestAgreementId = await _cache.GetOrAddAsync(GetCacheKeyForAccount(accountId), k => FetchLatestAgreementNumberFromStoreAsync(accountId));

            if (latestAgreementId == StoredValueThatMeansNull)
            {
                return null;
            }

            return latestAgreementId;
        }

        public Task RemoveFromCacheAsync(long accountId)
        {
            return _cache.RemoveFromCache(GetCacheKeyForAccount(accountId));
        }

        private string GetCacheKeyForAccount(long accountId)
        {
            return "Account:" + accountId.ToString(CultureInfo.InvariantCulture);
        }

        private async Task<int> FetchLatestAgreementNumberFromStoreAsync(long accountId)
        {
            return await _employerAgreementRepository.GetLatestSignedAgreementVersion(accountId) ?? StoredValueThatMeansNull;
        }
    }
}