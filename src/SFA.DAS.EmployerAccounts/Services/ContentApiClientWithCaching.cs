using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Services;

public class ContentApiClientWithCaching : IContentApiClient
{
    private readonly IContentApiClient _contentService;
    private readonly ICacheStorageService _cacheStorageService;
    private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

    public ContentApiClientWithCaching(IContentApiClient contentService, ICacheStorageService cacheStorageService, EmployerAccountsConfiguration employerAccountsConfiguration)
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
        catch(Exception ex)
        {
            throw new ArgumentException($"Failed to get content for {cacheKey}", ex);
        }
    }
}