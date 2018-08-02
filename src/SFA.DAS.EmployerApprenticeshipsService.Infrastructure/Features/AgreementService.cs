using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Caches;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class AgreementService : IAgreementService
    {
        private const int NullCacheValue = -1;

        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IDistributedCache _cache;

        public AgreementService(Lazy<EmployerAccountsDbContext> db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<int?> GetAgreementVersionAsync(long accountId)
        {
            var version = await _cache.GetOrAddAsync(GetCacheKeyForAccount(accountId), k => GetMinAgreementVersionAsync(accountId)).ConfigureAwait(false);

            if (version == NullCacheValue)
            {
                return null;
            }

            return version;
        }

        private async Task<int> GetMinAgreementVersionAsync(long accountId)
        {
            var versionNumber = await _db.Value.Agreements
                .Where(a => a.Account.Id == accountId && (a.StatusId == EmployerAgreementStatus.Pending || a.StatusId == EmployerAgreementStatus.Signed))
                .Select(a => new
                {
                    LegalEntityId = a.LegalEntity.Id,
                    TemplateVersion = a.StatusId == EmployerAgreementStatus.Signed ? a.Template.VersionNumber : 0
                })
                .GroupBy(x => x.LegalEntityId)
                .Select(g => g.Max(x => x.TemplateVersion))
                .MinAsync()
                .ConfigureAwait(false);

            return versionNumber == 0 ? NullCacheValue : versionNumber;
        }

        public Task RemoveFromCacheAsync(long accountId)
        {
            return _cache.RemoveFromCache(GetCacheKeyForAccount(accountId));
        }

        private string GetCacheKeyForAccount(long accountId)
        {
            return "AccountId:" + accountId.ToString(CultureInfo.InvariantCulture);
        }
    }
}