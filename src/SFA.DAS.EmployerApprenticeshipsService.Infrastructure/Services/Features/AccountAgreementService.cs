using System.Globalization;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class AccountAgreementService : IAccountAgreementService
    {
        // value stored in cache as proxy for null (we can't store nulls in the cache)
        private const int StoredValueThatMeansNull = -1;

        private readonly IDistributedCache _cache;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public AccountAgreementService(
            IDistributedCache cache,
            IEmployerAgreementRepository employerAgreementRepository)
        {
            _cache = cache;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<int?> GetLatestAgreementSignedByAccountAsync(long accountId)
        {
            var latestAgreementId = await _cache
                .GetOrAddAsync(GetCacheKeyForAccount(accountId), 
                            key => FetchLatestAgreeemntNumberFromStoreAsync(accountId));

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

        private async Task<int> FetchLatestAgreeemntNumberFromStoreAsync(long accountId)
        {
            return await _employerAgreementRepository.GetLatestSignedAgreementVersion(accountId) ?? StoredValueThatMeansNull;
        }

        private string GetCacheKeyForAccount(long accountId)
        {
            return "Account:"+accountId.ToString(CultureInfo.InvariantCulture);
        }
    }
}