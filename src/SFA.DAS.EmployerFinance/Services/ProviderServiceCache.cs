using SFA.DAS.Caches;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ProviderServiceCache : IProviderService
    {
        private readonly IInProcessCache _inProcessCache;
        private readonly IProviderService _providerService;

        public ProviderServiceCache(IProviderService providerService, IInProcessCache inProcessCache)
        {
            _providerService = providerService;
            _inProcessCache = inProcessCache;
        }

        public async Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn)
        {
            var cachedProvider = _inProcessCache.Get<Models.ApprenticeshipProvider.Provider>($"{nameof(Models.ApprenticeshipProvider.Provider)}_{ukPrn}");

            if (cachedProvider != null)
            {
                return cachedProvider;
            }

            var provider = await _providerService.Get(ukPrn);

            if (provider != null)
            {
                _inProcessCache.Set($"{nameof(Models.ApprenticeshipProvider.Provider)}_{ukPrn}", provider, new TimeSpan(1, 0, 0));
            }

            return provider;           
        }
    }
}
