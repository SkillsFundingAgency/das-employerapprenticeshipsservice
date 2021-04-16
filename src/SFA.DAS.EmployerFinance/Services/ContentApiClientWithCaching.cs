using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ContentApiClientWithCaching : IContentApiClient
    {
        private readonly IContentApiClient _contentApiClient;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerFinanceConfiguration _employerFinanceConfiguration;

        public ContentApiClientWithCaching(IContentApiClient contentApiClient, ICacheStorageService cacheStorageService, EmployerFinanceConfiguration employerFinanceConfiguration)
        {
            _contentApiClient = contentApiClient;
            _cacheStorageService = cacheStorageService;
            _employerFinanceConfiguration = employerFinanceConfiguration;
        }
        public async Task<string> Get(string type, string applicationId)
        {
            var cacheKey = $"{applicationId}_{type}".ToLowerInvariant();

            try
            {
                if (_cacheStorageService.TryGet(cacheKey, out string cachedContentBanner))
                {
                    return cachedContentBanner;
                }

                var content = await _contentApiClient.Get(type, applicationId);

                if (content != null)
                {
                    await _cacheStorageService.Save(cacheKey, content, _employerFinanceConfiguration.DefaultCacheExpirationInMinutes);
                }

                return content;
            }
            catch
            {
                throw new ArgumentException($"Failed to get content for {cacheKey}");
            }
        }
    }
}
