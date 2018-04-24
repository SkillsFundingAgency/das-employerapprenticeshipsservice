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

        public async Task<int?> GetLatestSignedAgreementVersionAsync(long accountId)
        {
            var version = await _cache.GetOrAddAsync(GetCacheKeyForAccount(accountId), k => FetchLatestAgreementNumberFromStoreAsync(accountId));

            if (version == StoredValueThatMeansNull)
            {
                return null;
            }

            return version;
        }

        public Task RemoveFromCacheAsync(long accountId)
        {
            return _cache.RemoveFromCache(GetCacheKeyForAccount(accountId));
        }

        private async Task<int> FetchLatestAgreementNumberFromStoreAsync(long accountId)
        {
            return await _employerAgreementRepository.GetLatestSignedAgreementVersion(accountId) ?? StoredValueThatMeansNull;
        }

        private string GetCacheKeyForAccount(long accountId)
        {
            return "Account:" + accountId.ToString(CultureInfo.InvariantCulture);
        }
    }
}