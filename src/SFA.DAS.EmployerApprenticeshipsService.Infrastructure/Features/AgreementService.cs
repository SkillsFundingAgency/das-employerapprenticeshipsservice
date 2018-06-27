using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.Features
{
    public class AgreementService : IAgreementService
    {
        private const int NullCacheValue = -1;

        private readonly EmployerAccountDbContext _db;
        private readonly IDistributedCache _cache;

        public AgreementService(EmployerAccountDbContext db, IDistributedCache cache)
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
            var versionNumber = await _db.AccountLegalEntity
                                        .Where(ale => ale.AccountId == accountId)
                                        .MinAsync(ale => ale.SignedAgreementId == null ? 0 : (int) ale.SignedAgreementVersion)
                                        .ConfigureAwait(false);

            return versionNumber > 0 ? versionNumber : NullCacheValue;
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