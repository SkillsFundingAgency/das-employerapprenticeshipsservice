using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ClientContentServiceWithCaching : IClientContentService
    {
        private readonly IClientContentService _contentService;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerFinanceConfiguration _employerFinanceConfiguration;

        public ClientContentServiceWithCaching(IClientContentService contentService, ICacheStorageService cacheStorageService, EmployerFinanceConfiguration employerFinanceConfiguration)
        {
            _contentService = contentService;
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

                var content = await _contentService.Get(type, applicationId);

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
