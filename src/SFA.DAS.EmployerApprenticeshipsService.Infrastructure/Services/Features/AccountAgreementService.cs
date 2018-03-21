using System;
using System.Globalization;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.Services.Features
{
    public class AccountAgreementService : IAccountAgreementService
    {
        // value stored in memcache as proxy for null
        private const decimal StoredValueThatMeansNull = -1;

        private readonly ICacheProvider _cacheProvider;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public AccountAgreementService(
            ICacheProvider cacheProvider,
            IEmployerAgreementRepository employerAgreementRepository)
        {
            _cacheProvider = cacheProvider;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<decimal?> GetLatestAgreementSignedByAccountAsync(long accountId)
        {
            var latestAgreementId = await _cacheProvider
                .GetOrAdd(accountId.ToString(CultureInfo.InvariantCulture), 
                            key => FetchLatestAgreeemntNumberFromStoreAsync(accountId),
                            DateTimeOffset.Now.AddMinutes(30L));

            if (latestAgreementId == StoredValueThatMeansNull)
            {
                return null;
            }

            return latestAgreementId;
        }

        private async Task<decimal?> FetchLatestAgreeemntNumberFromStoreAsync(long accountId)
        {
            return await _employerAgreementRepository.GetLatestSignedAgreementVersion(accountId) ?? StoredValueThatMeansNull;
        }
    }
}