using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ContentServiceWithCaching : IContentService
    {
        private readonly IContentService _contentService;
        private readonly ICacheStorageService _cacheStorageService;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

        public ContentServiceWithCaching(IContentService contentService, ICacheStorageService cacheStorageService, EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _contentService = contentService;
            _cacheStorageService = cacheStorageService;
            _employerAccountsConfiguration = employerAccountsConfiguration;
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
                    await _cacheStorageService.Save(cacheKey, content, _employerAccountsConfiguration.DefaultCacheExpirationInMinutes);
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
